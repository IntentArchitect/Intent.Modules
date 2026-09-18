namespace Intent.Agent.Gate;

public enum VersionScheme
{
    Final,
    PreRelease,
}

/// <summary>
/// Command routing. Takes stdin/stdout/stderr and the working directory explicitly (rather
/// than reading Console directly) so tests can drive every command end-to-end against fakes.
///
/// Every harness this gate targets honours exit code 2 as "block" and 0 as "allow" (confirmed
/// against Claude Code, Codex, Kiro and Cursor's own docs - Cursor specifically treats any
/// OTHER non-zero code as an allow, so a crash must become an explicit exit 2, never the
/// runtime's default). guard-write and guard-version can deny; close-out never does - it only
/// ever informs, per the module's deny/warn split - so it always exits 0.
/// </summary>
public static class Cli
{
    public static int Run(
        string[] args,
        TextReader stdin,
        TextWriter stdout,
        TextWriter stderr,
        string currentDirectory,
        IGitChangeProvider? gitChangeProvider = null)
    {
        try
        {
            if (args.Length == 0)
            {
                stderr.WriteLine("Usage: gate.cs <guard-write|guard-version|close-out|warm> [--harness claude|codex|cursor|kiro] [--scheme pre|final] [path]");
                return 2;
            }

            var command = args[0];
            var rest = args[1..];
            var repoRoot = GitRepoLocator.FindRepoRoot(currentDirectory);

            return command switch
            {
                "warm" => 0,
                "guard-write" => RunGuardWrite(rest, stdin, stdout, stderr, repoRoot),
                "guard-version" => RunGuardVersion(rest, stdin, stdout, stderr, repoRoot, gitChangeProvider ?? new ProcessGitChangeProvider()),
                "close-out" => RunCloseOut(rest, stdout, repoRoot, gitChangeProvider ?? new ProcessGitChangeProvider()),
                _ => Unknown(command, stderr),
            };
        }
        catch (Exception ex)
        {
            // Fail closed on a genuine crash. This must be an EXPLICIT exit(2), not whatever
            // the runtime's default unhandled-exception code is: Cursor specifically treats
            // any non-zero code other than 2 as an allow.
            stderr.WriteLine($"intent-agent-gate: unexpected error, blocking rather than guessing: {ex.Message}");
            return 2;
        }
    }

    private static int RunGuardWrite(string[] rest, TextReader stdin, TextWriter stdout, TextWriter stderr, string? repoRoot)
    {
        var (harness, positional) = ParseArgs(rest);
        var input = stdin.ReadToEnd();
        var path = StdinPathExtractor.Extract(input, positional);

        if (repoRoot is null || path is null)
        {
            return Allow(harness, stdout, repoRoot is null
                ? "intent-agent-gate: could not locate the git repository root; no opinion."
                : "intent-agent-gate: could not determine a target file path from stdin or arguments; no opinion.");
        }

        // Never hand-edited, for any reason - it records what is installed, and a bad
        // edit corrupts the application's module state.
        if (string.Equals(Path.GetFileName(path), "modules.config", StringComparison.OrdinalIgnoreCase))
        {
            return Deny(harness, stdout, stderr,
                "modules.config is never hand-edited. It records what is installed; change it by installing " +
                "or updating through Intent Architect, not by editing the file.");
        }

        // ".imodspec" is field-scoped, not file-scoped: <tags>, <dependency> and a version
        // downgrade are legitimate hand-edits; <summary>/<description> are always denied,
        // because the Software Factory overwrites both from Application Settings on every
        // run and discards a manual edit silently.
        if (path.EndsWith(".imodspec", StringComparison.OrdinalIgnoreCase))
        {
            var edit = StdinEditExtractor.Extract(input);
            var editedText = edit.WholeFileContent ?? edit.EditSnippet;
            if (editedText is null)
            {
                return Allow(harness, stdout, "intent-agent-gate: could not read the edit content; no opinion.");
            }

            string? currentDiskContent = null;
            if (edit.WholeFileContent is not null && File.Exists(path))
            {
                try
                {
                    currentDiskContent = File.ReadAllText(path);
                }
                catch (IOException)
                {
                    // Fall through to the snippet-only comparison below.
                }
            }

            if (ImodspecFieldGuard.IsHandEditAllowed(currentDiskContent, editedText))
            {
                return Allow(harness, stdout);
            }

            return Deny(harness, stdout, stderr,
                "<summary> and <description> in .imodspec are overwritten by the Software Factory from " +
                "Application Settings on every run - a hand-edit here is discarded silently, not just " +
                "denied. Change the summary/description on the Application Settings page instead. " +
                "<tags>, <dependency> entries and a version downgrade remain fine to hand-edit.");
        }

        var match = ManagedFilesGuard.FindMatch(repoRoot, path);
        if (match is null)
        {
            // The overwhelming common case - stay completely silent, zero tokens.
            return Allow(harness, stdout);
        }

        var relativeManagedFiles = Path.GetRelativePath(repoRoot, match.ManagedFilesXmlPath).Replace('\\', '/');
        var reason =
            "Never edit generated output to make a regeneration look correct. This path is Software-Factory-owned " +
            $"output generated by template '{match.TemplateId}' (listed in {relativeManagedFiles}). Change the " +
            "designer model and regenerate instead.";
        return Deny(harness, stdout, stderr, reason);
    }

    private static int RunGuardVersion(string[] rest, TextReader stdin, TextWriter stdout, TextWriter stderr, string? repoRoot, IGitChangeProvider gitChangeProvider)
    {
        var (harness, scheme) = ParseGuardVersionArgs(rest);
        var input = stdin.ReadToEnd();
        var invocation = DesignerScriptInputExtractor.Extract(input);

        if (invocation.Script is null || VersionScriptExtractor.ExtractNewVersion(invocation.Script) is not { } newVersionText)
        {
            // Not a Version-setting script at all - silent allow, zero tokens.
            return Allow(harness, stdout);
        }

        if (repoRoot is null || invocation.ApplicationId is null)
        {
            return Allow(harness, stdout, "intent-agent-gate: could not resolve the target module; no opinion.");
        }

        var moduleFolder = ModuleResolver.FindModuleFolderByApplicationId(repoRoot, invocation.ApplicationId);
        var imodspecPath = moduleFolder is null ? null : ModuleResolver.FindImodspec(moduleFolder);
        if (imodspecPath is null)
        {
            return Allow(harness, stdout, "intent-agent-gate: could not locate the module's .imodspec; no opinion.");
        }

        if (!SemVer.TryParse(newVersionText, out var newVersion))
        {
            return Deny(harness, stdout, stderr,
                $"'{newVersionText}' is not a valid semantic version (expected Major.Minor.Patch[-pre.N]).");
        }

        var currentVersionText = ModuleResolver.ReadVersionFromImodspecFile(imodspecPath);
        if (currentVersionText is null || !SemVer.TryParse(currentVersionText, out var currentVersion))
        {
            return Allow(harness, stdout, "intent-agent-gate: could not read the module's current version; no opinion.");
        }

        // Downgrade guard - the new value must sort strictly higher than what is on disk now.
        if (SemVer.ComparePrecedence(newVersion, currentVersion) <= 0)
        {
            return Deny(harness, stdout, stderr,
                $"'{newVersionText}' does not sort strictly higher than the current version '{currentVersionText}'. " +
                "If this is a deliberate downgrade correction, the Software Factory will not regenerate a lower value " +
                "from the designer - set the version directly in the .imodspec instead (see module-version-increment).");
        }

        // Double-bump guard - has this module's version already moved vs HEAD in this line of work?
        var relativeImodspec = Path.GetRelativePath(repoRoot, imodspecPath).Replace('\\', '/');
        var headContent = gitChangeProvider.TryReadFileAtHead(repoRoot, relativeImodspec);
        var headVersionText = headContent is null ? null : ModuleResolver.ReadVersionFromImodspecContent(headContent);
        if (headVersionText is not null &&
            SemVer.TryParse(headVersionText, out var headVersion) &&
            SemVer.ComparePrecedence(currentVersion, headVersion) > 0)
        {
            return Deny(harness, stdout, stderr,
                $"This module's version already moved this line of work ('{headVersionText}' -> '{currentVersionText}'). " +
                "A version moves once per line of work - see module-version-increment. If more work landed since, " +
                "that is the same bump, not a second one.");
        }

        // Scheme guard - under pre-release mode, dropping the "-pre.N" suffix is only legal
        // as a promotion of the SAME core version; introducing a new core version bare is
        // the violation.
        var sameCore = newVersion.Major == currentVersion.Major
            && newVersion.Minor == currentVersion.Minor
            && newVersion.Patch == currentVersion.Patch;
        if (scheme == VersionScheme.PreRelease && newVersion.PreRelease is null && !sameCore)
        {
            return Deny(harness, stdout, stderr,
                $"Pre-release versioning is configured, so '{newVersionText}' should be " +
                $"'{newVersion.Major}.{newVersion.Minor}.{newVersion.Patch}-pre.0'.");
        }

        return Allow(harness, stdout);
    }

    private static int RunCloseOut(string[] rest, TextWriter stdout, string? repoRoot, IGitChangeProvider gitChangeProvider)
    {
        var (harness, _) = ParseArgs(rest);

        if (repoRoot is null)
        {
            // Nothing to check outside a git repository - silent, not an error.
            return 0;
        }

        var changedFiles = gitChangeProvider.GetChangedFiles(repoRoot);
        var versionResult = ModuleVersionAuditor.Audit(repoRoot, changedFiles);
        var otherFindings = CloseOutAuditor.Audit(repoRoot, changedFiles);

        if (versionResult.Passed && otherFindings.Count == 0)
        {
            // Clean - silent, zero tokens. Close-out never blocks; see the deny/warn split.
            return 0;
        }

        var messages = new List<string>();
        if (!versionResult.Passed)
        {
            var moduleNames = string.Join(", ", versionResult.Violations.Select(v => v.ModuleName));
            messages.Add(
                $"Module(s) changed without their .imodspec version moving: {moduleNames}. " +
                "Consult the module-version-increment skill before finishing this change.");
        }

        messages.AddRange(otherFindings.Select(f => $"[{f.ModuleName}] {f.Message}"));

        stdout.WriteLine(HarnessProtocol.FormatWarning(harness, string.Join(" ", messages)));
        return 0;
    }

    private static int Allow(Harness harness, TextWriter stdout, string? reason = null)
    {
        if (reason is not null)
        {
            var formatted = HarnessProtocol.FormatToolPermission(harness, allow: true, reason);
            if (formatted is not null)
            {
                stdout.WriteLine(formatted);
            }
        }

        return 0;
    }

    private static int Deny(Harness harness, TextWriter stdout, TextWriter stderr, string reason)
    {
        var formatted = HarnessProtocol.FormatToolPermission(harness, allow: false, reason);
        if (formatted is not null)
        {
            stdout.WriteLine(formatted);
        }

        stderr.WriteLine(reason);
        return 2;
    }

    private static int Unknown(string command, TextWriter stderr)
    {
        stderr.WriteLine($"Unknown command: {command}");
        return 2;
    }

    private static (Harness Harness, string? Positional) ParseArgs(string[] rest)
    {
        string? harnessValue = null;
        string? positional = null;

        for (var i = 0; i < rest.Length; i++)
        {
            if (rest[i] == "--harness" && i + 1 < rest.Length)
            {
                harnessValue = rest[i + 1];
                i++;
            }
            else if (rest[i] == "--scheme" && i + 1 < rest.Length)
            {
                i++;
            }
            else if (!rest[i].StartsWith("--", StringComparison.Ordinal))
            {
                positional ??= rest[i];
            }
        }

        return (HarnessProtocol.Parse(harnessValue), positional);
    }

    private static (Harness Harness, VersionScheme Scheme) ParseGuardVersionArgs(string[] rest)
    {
        var (harness, _) = ParseArgs(rest);

        var scheme = VersionScheme.Final;
        for (var i = 0; i < rest.Length; i++)
        {
            if (rest[i] == "--scheme" && i + 1 < rest.Length && rest[i + 1] == "pre")
            {
                scheme = VersionScheme.PreRelease;
            }
        }

        return (harness, scheme);
    }
}