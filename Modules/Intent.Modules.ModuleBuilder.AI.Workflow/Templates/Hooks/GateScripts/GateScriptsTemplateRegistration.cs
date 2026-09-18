using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.GateScripts
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class GateScriptsTemplateRegistration : FilePerModelTemplateRegistration<GateSourceFileModel>
    {
        private readonly IMetadataManager _metadataManager;

        public GateScriptsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => GateScriptsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, GateSourceFileModel model)
        {
            return new GateScriptsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<GateSourceFileModel> GetModels(IApplication application)
        {
            if (!AgentGateSwitch.IsOn(application))
            {
                yield break;
            }

            yield return new GateSourceFileModel("Directory.Build", "props", DirectoryBuildPropsContent);
            yield return new GateSourceFileModel("SemVer", "cs", SemVerContent);
            yield return new GateSourceFileModel("HarnessProtocol", "cs", HarnessProtocolContent);
            yield return new GateSourceFileModel("GitSupport", "cs", GitSupportContent);
            yield return new GateSourceFileModel("IntentMetadataGuard", "cs", IntentMetadataGuardContent);
            yield return new GateSourceFileModel("ModuleVersionAuditor", "cs", ModuleVersionAuditorContent);
            yield return new GateSourceFileModel("CloseOutAuditor", "cs", CloseOutAuditorContent);
            yield return new GateSourceFileModel("GuardVersionSupport", "cs", GuardVersionSupportContent);
            yield return new GateSourceFileModel("Cli", "cs", CliContent);
            yield return new GateSourceFileModel("gate", "cs", GateEntryPointContent);
        }

        // Shields the file-based app from the consuming repo's own root Directory.Build.props,
        // which would otherwise apply here too (Microsoft's own documented mitigation).
        private const string DirectoryBuildPropsContent = """
            <Project>
            </Project>
            """;

        // #:include-d by gate.cs. Top-level statements can only live in the entry point itself, so
        // every other file here holds classes only.
        private const string GateEntryPointContent = """
            #:property PublishAot=false
            #:include SemVer.cs
            #:include HarnessProtocol.cs
            #:include GitSupport.cs
            #:include IntentMetadataGuard.cs
            #:include ModuleVersionAuditor.cs
            #:include CloseOutAuditor.cs
            #:include GuardVersionSupport.cs
            #:include Cli.cs

            using Intent.Agent.Gate;

            return Cli.Run(args, Console.In, Console.Out, Console.Error, Directory.GetCurrentDirectory());
            """;

        private const string SemVerContent = """
            namespace Intent.Agent.Gate;

            /// <summary>
            /// A parsed semantic version. Comparison follows semver PRECEDENCE, not the version's own
            /// string form - the trap this exists to avoid is that "1.0.0-pre.10" sorts ABOVE
            /// "1.0.0-pre.9" (pre-release identifiers that are both numeric compare numerically), while a
            /// naive string comparison would put "pre.10" first.
            /// </summary>
            public readonly record struct SemVer(int Major, int Minor, int Patch, string? PreRelease)
            {
                public static bool TryParse(string value, out SemVer version)
                {
                    version = default;
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return false;
                    }

                    var dashIndex = value.IndexOf('-');
                    var core = dashIndex >= 0 ? value[..dashIndex] : value;
                    var preRelease = dashIndex >= 0 ? value[(dashIndex + 1)..] : null;

                    var parts = core.Split('.');
                    if (parts.Length != 3
                        || !int.TryParse(parts[0], out var major)
                        || !int.TryParse(parts[1], out var minor)
                        || !int.TryParse(parts[2], out var patch))
                    {
                        return false;
                    }

                    version = new SemVer(major, minor, patch, string.IsNullOrEmpty(preRelease) ? null : preRelease);
                    return true;
                }

                /// <summary>
                /// Semver precedence comparison. A pre-release sorts BELOW the same core version with no
                /// suffix (so "1.0.0-pre.9" -> "1.0.0" is a legal promotion, not a violation). Pre-release
                /// identifiers compare numerically when both sides of a dot-segment are numeric, and
                /// lexically otherwise - per the semver spec, not a plain string comparison.
                /// </summary>
                public static int ComparePrecedence(SemVer a, SemVer b)
                {
                    var core = a.Major.CompareTo(b.Major);
                    if (core != 0)
                    {
                        return core;
                    }

                    core = a.Minor.CompareTo(b.Minor);
                    if (core != 0)
                    {
                        return core;
                    }

                    core = a.Patch.CompareTo(b.Patch);
                    if (core != 0)
                    {
                        return core;
                    }

                    if (a.PreRelease is null && b.PreRelease is null)
                    {
                        return 0;
                    }

                    if (a.PreRelease is null)
                    {
                        return 1;
                    }

                    if (b.PreRelease is null)
                    {
                        return -1;
                    }

                    return ComparePreReleaseIdentifiers(a.PreRelease, b.PreRelease);
                }

                private static int ComparePreReleaseIdentifiers(string a, string b)
                {
                    var aParts = a.Split('.');
                    var bParts = b.Split('.');
                    var length = Math.Min(aParts.Length, bParts.Length);

                    for (var i = 0; i < length; i++)
                    {
                        var aIsNumeric = int.TryParse(aParts[i], out var aNum);
                        var bIsNumeric = int.TryParse(bParts[i], out var bNum);

                        int cmp;
                        if (aIsNumeric && bIsNumeric)
                        {
                            cmp = aNum.CompareTo(bNum);
                        }
                        else if (aIsNumeric != bIsNumeric)
                        {
                            // A numeric identifier always has lower precedence than an alphanumeric one.
                            cmp = aIsNumeric ? -1 : 1;
                        }
                        else
                        {
                            cmp = string.CompareOrdinal(aParts[i], bParts[i]);
                        }

                        if (cmp != 0)
                        {
                            return cmp;
                        }
                    }

                    return aParts.Length.CompareTo(bParts.Length);
                }

                public override string ToString() => PreRelease is null ? $"{Major}.{Minor}.{Patch}" : $"{Major}.{Minor}.{Patch}-{PreRelease}";
            }
            """;

        private const string HarnessProtocolContent = """
            using System.Text.Json;

            namespace Intent.Agent.Gate;

            /// <summary>
            /// Which AI harness is asking. Claude Code and Codex share a JSON dialect for a
            /// PreToolUse-style decision (confirmed from vendor docs), so they are one case here.
            /// </summary>
            public enum Harness
            {
                ClaudeCodeOrCodex,
                Cursor,
                Kiro
            }

            /// <summary>
            /// Serializes a gate decision into the harness-specific shape its hook runner expects. Every
            /// harness this gate targets blocks via exit code 2 - that is the one universal signal - and
            /// this class only decides what, if anything, additionally goes to stdout so a human or the
            /// model can read the reason. If real hook wiring shows a different shape, only this file
            /// needs to change.
            ///
            /// Confirmed from vendor documentation:
            ///  - Claude Code / Codex: "hookSpecificOutput.permissionDecision" / "permissionDecisionReason"
            ///    for a PreToolUse-style decision; Claude Code's Stop hook reads "systemMessage" for a
            ///    non-blocking note.
            ///  - Cursor: exit 2 blocks regardless of the JSON body; the body is
            ///    "permission": "allow"|"deny", "agent_message". Cursor fails OPEN on any other non-zero
            ///    exit code, so the caller must always exit exactly 0 or 2, never the runtime's default
            ///    unhandled-exception code.
            ///  - Kiro: blocks purely via exit code 2 plus the reason on stderr - no JSON body is read for
            ///    a PreToolUse deny. On a successful (exit 0) hook, Kiro incorporates stdout directly into
            ///    the agent's context, so a close-out warning is written there as plain text, not JSON.
            /// </summary>
            public static class HarnessProtocol
            {
                public static Harness Parse(string? value) => value?.Trim().ToLowerInvariant() switch
                {
                    "cursor" => Harness.Cursor,
                    "kiro" => Harness.Kiro,
                    _ => Harness.ClaudeCodeOrCodex,
                };

                /// <summary>
                /// PreToolUse-equivalent decision, used by guard-write and guard-version. Returns what to
                /// write to stdout, or null when nothing should be written there (Kiro - stderr alone
                /// carries the reason on a deny).
                /// </summary>
                public static string? FormatToolPermission(Harness harness, bool allow, string reason) => harness switch
                {
                    Harness.Kiro => null,
                    Harness.Cursor => Serialize(new { permission = allow ? "allow" : "deny", agent_message = reason }),
                    _ => Serialize(new
                    {
                        hookSpecificOutput = new
                        {
                            hookEventName = "PreToolUse",
                            permissionDecision = allow ? "allow" : "deny",
                            permissionDecisionReason = reason,
                        },
                    }),
                };

                /// <summary>
                /// Stop-equivalent, NON-BLOCKING note used by close-out. Close-out never denies (see the
                /// module's deny/warn split), so this only decides how the reason is surfaced when there
                /// is one - callers always exit 0 regardless of what this returns.
                /// </summary>
                public static string FormatWarning(Harness harness, string reason) => harness switch
                {
                    Harness.Kiro => reason,
                    Harness.Cursor => Serialize(new { permission = "allow", agent_message = reason }),
                    _ => Serialize(new { systemMessage = reason }),
                };

                private static string Serialize(object payload) => JsonSerializer.Serialize(payload);
            }
            """;

        private const string GitSupportContent = """
            using System.Diagnostics;
            using System.Text.Json;

            namespace Intent.Agent.Gate;

            /// <summary>
            /// Finds the root of the git repository (or worktree) containing a given directory, by
            /// walking upward looking for a ".git" entry. In a worktree, ".git" is a file (pointing at
            /// the main repository's worktree metadata) rather than a directory, so both are treated as
            /// a match.
            /// </summary>
            public static class GitRepoLocator
            {
                public static string? FindRepoRoot(string startDirectory)
                {
                    var current = new DirectoryInfo(startDirectory).FullName;

                    while (true)
                    {
                        var dotGit = Path.Combine(current, ".git");
                        if (Directory.Exists(dotGit) || File.Exists(dotGit))
                        {
                            return current;
                        }

                        var parent = Directory.GetParent(current);
                        if (parent is null)
                        {
                            return null;
                        }

                        current = parent.FullName;
                    }
                }
            }

            /// <summary>
            /// Abstraction over "what files changed" and "what a file looked like at HEAD", so gate
            /// logic can be unit-tested against a fake without shelling out to git.
            /// </summary>
            public interface IGitChangeProvider
            {
                /// <summary>
                /// Paths (relative to <paramref name="repoRoot"/>, forward-slash separated, matching
                /// git's own output convention) of files that differ between the working tree and HEAD.
                /// </summary>
                IReadOnlyList<string> GetChangedFiles(string repoRoot);

                /// <summary>
                /// The content of <paramref name="relativePath"/> as it was at HEAD, or null if the path
                /// did not exist at HEAD (e.g. a newly-added file, or a module newly created in this
                /// line of work).
                /// </summary>
                string? TryReadFileAtHead(string repoRoot, string relativePath);
            }

            /// <summary>
            /// Real <see cref="IGitChangeProvider"/> that shells out to git.
            /// </summary>
            public sealed class ProcessGitChangeProvider : IGitChangeProvider
            {
                public IReadOnlyList<string> GetChangedFiles(string repoRoot)
                {
                    // "git diff" alone misses brand-new (untracked) files entirely - a module
                    // created from scratch in this line of work would otherwise be invisible to
                    // the audit below. Union tracked changes against HEAD with untracked files.
                    var changed = RunGit(repoRoot, "diff", "--name-only", "HEAD");
                    var untracked = RunGit(repoRoot, "ls-files", "--others", "--exclude-standard");

                    return SplitLines(changed).Concat(SplitLines(untracked)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                }

                private static IEnumerable<string> SplitLines(string? output) =>
                    output is null
                        ? []
                        : output.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0);

                public string? TryReadFileAtHead(string repoRoot, string relativePath)
                {
                    return RunGit(repoRoot, "show", $"HEAD:{relativePath}");
                }

                private static string? RunGit(string repoRoot, params string[] args)
                {
                    var startInfo = new ProcessStartInfo("git")
                    {
                        WorkingDirectory = repoRoot,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                    };
                    foreach (var arg in args)
                    {
                        startInfo.ArgumentList.Add(arg);
                    }

                    using var process = Process.Start(startInfo);
                    if (process is null)
                    {
                        return null;
                    }

                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // A non-zero exit (e.g. "show HEAD:path" for a path that did not exist at HEAD)
                    // means no content to read, not an error worth surfacing to the caller.
                    return process.ExitCode == 0 ? output : null;
                }
            }

            /// <summary>
            /// Extracts a target file path from a PreToolUse-style hook payload on stdin. The exact
            /// JSON shape differs per harness (Claude Code / Codex nest it under "tool_input"; Cursor's
            /// and Kiro's exact shapes are not confirmed from documentation), so this does a tolerant
            /// search: try a short list of candidate key names at the top level, then one level of
            /// nesting inside any object property. A positional CLI argument is accepted as a fallback,
            /// for manual testing or a harness whose payload this doesn't recognise.
            /// </summary>
            public static class StdinPathExtractor
            {
                private static readonly string[] CandidateKeys = ["file_path", "filePath", "path"];

                public static string? Extract(string input, string? positionalArg = null)
                {
                    var fromJson = TryExtractFromJson(input);
                    if (!string.IsNullOrWhiteSpace(fromJson))
                    {
                        return fromJson;
                    }

                    return string.IsNullOrWhiteSpace(positionalArg) ? null : positionalArg;
                }

                private static string? TryExtractFromJson(string input)
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return null;
                    }

                    try
                    {
                        using var document = JsonDocument.Parse(input);
                        var root = document.RootElement;

                        if (root.ValueKind != JsonValueKind.Object)
                        {
                            return null;
                        }

                        foreach (var key in CandidateKeys)
                        {
                            if (TryGetString(root, key, out var value))
                            {
                                return value;
                            }
                        }

                        foreach (var property in root.EnumerateObject())
                        {
                            if (property.Value.ValueKind != JsonValueKind.Object)
                            {
                                continue;
                            }

                            foreach (var key in CandidateKeys)
                            {
                                if (TryGetString(property.Value, key, out var value))
                                {
                                    return value;
                                }
                            }
                        }

                        return null;
                    }
                    catch (JsonException)
                    {
                        return null;
                    }
                }

                private static bool TryGetString(JsonElement element, string propertyName, out string value)
                {
                    value = string.Empty;

                    if (element.ValueKind == JsonValueKind.Object &&
                        element.TryGetProperty(propertyName, out var property) &&
                        property.ValueKind == JsonValueKind.String)
                    {
                        var raw = property.GetString();
                        if (!string.IsNullOrWhiteSpace(raw))
                        {
                            value = raw;
                            return true;
                        }
                    }

                    return false;
                }
            }
            """;

        private const string IntentMetadataGuardContent = """
            namespace Intent.Agent.Gate;

            /// <summary>
            /// Intent Architect's own METADATA: the designer model, an application's configuration, and
            /// the record of what it generated and installed. None of it is hand-edited.
            /// </summary>
            /// <remarks>
            /// This is a different category from generated OUTPUT, and the distinction is the whole
            /// point of the guard. Editing generated output is usually merely futile - the next run
            /// overwrites it. Editing metadata CORRUPTS state the Software Factory depends on, and no
            /// regeneration puts it right.
            /// <para>
            /// It is also high precision by construction: every legitimate change here has a proper
            /// mechanism - the Intent MCP server, or a skill that drives it - so a denial can always
            /// say where to go instead of merely saying no. An earlier version of this guard blocked
            /// generated output instead, and in real use produced false positive after false positive:
            /// authoring a scaffolded template, writing release notes and correcting a csproj package
            /// version are all intended workflows, and all were denied.
            /// </para>
            /// </remarks>
            public static class IntentMetadataGuard
            {
                /// <summary>Records what is installed; a bad edit corrupts the application's module state.</summary>
                private const string ModulesConfig = "modules.config";

                /// <summary>
                /// Matched as suffixes rather than exact names, because each is prefixed with the
                /// owning application's name - "MyApp.application.config", and so on.
                /// </summary>
                private static readonly string[] ProtectedSuffixes =
                {
                    ".application.config",
                    ".application.managed-files.xml",
                    ".application.output.config.xml",
                    ".application.deviations.log.xml",
                };

                /// <summary>Any file anywhere beneath one of these is designer-owned model content.</summary>
                private static readonly string[] ProtectedDirectories =
                {
                    "Intent.Metadata",
                    ".intent",
                };

                /// <summary>
                /// The reason this path must not be hand-edited, or null when it is not Intent metadata.
                /// </summary>
                public static string? DescribeViolation(string path)
                {
                    var fileName = Path.GetFileName(path);

                    if (string.Equals(fileName, ModulesConfig, StringComparison.OrdinalIgnoreCase))
                    {
                        return "modules.config records what is installed and is never hand-edited. Install or "
                             + "update the module through Intent Architect instead - the Intent MCP server's "
                             + "install_or_update_modules, or the equivalent action in the UI.";
                    }

                    foreach (var suffix in ProtectedSuffixes)
                    {
                        if (fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                        {
                            return $"'{fileName}' is Intent Architect application metadata, owned by the designers "
                                 + "and the Software Factory. Hand-editing it corrupts state that no regeneration "
                                 + "puts right. Change it through the Intent MCP server, or a skill that drives it.";
                        }
                    }

                    var segments = path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var directory in ProtectedDirectories)
                    {
                        if (Array.Exists(segments, segment => string.Equals(segment, directory, StringComparison.OrdinalIgnoreCase)))
                        {
                            return $"This path is inside '{directory}', which holds Intent Architect's designer "
                                 + "model. It is never edited directly. Use the Intent MCP server - "
                                 + "run_designer_script to change the model - or a skill that drives it.";
                        }
                    }

                    return null;
                }
            }
            """;

        private const string ModuleVersionAuditorContent = """
            namespace Intent.Agent.Gate;

            /// <summary>
            /// A module under "Modules/&lt;ModuleName&gt;/" whose files changed without its .imodspec
            /// (the file carrying its version) also changing.
            /// </summary>
            public sealed record ModuleVersionViolation(string ModuleName, string ImodspecRelativePath);

            public sealed record ModuleVersionAuditResult(bool Passed, IReadOnlyList<ModuleVersionViolation> Violations);

            /// <summary>
            /// The close-out check: for every module whose files changed (relative to HEAD), did that
            /// module's own .imodspec also change? This proves only that the version line moved, not
            /// that the chosen component (major/minor/patch) was correct - that judgement needs a
            /// module-feed query this gate deliberately does not attempt, which is exactly why this is a
            /// warning rather than a deny.
            /// </summary>
            public static class ModuleVersionAuditor
            {
                public static ModuleVersionAuditResult Audit(string repoRoot, IReadOnlyList<string> changedFiles)
                {
                    var normalizedChanged = changedFiles
                        .Select(Normalize)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var modulesTouched = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var file in normalizedChanged)
                    {
                        var moduleName = TryGetModuleName(file);
                        if (moduleName is not null)
                        {
                            modulesTouched.Add(moduleName);
                        }
                    }

                    var violations = new List<ModuleVersionViolation>();
                    var modulesDir = Path.Combine(repoRoot, "Modules");

                    foreach (var moduleName in modulesTouched)
                    {
                        var moduleDir = Path.Combine(modulesDir, moduleName);
                        if (!Directory.Exists(moduleDir))
                        {
                            continue;
                        }

                        var imodspecFiles = Directory.EnumerateFiles(moduleDir, "*.imodspec", SearchOption.AllDirectories).ToList();
                        if (imodspecFiles.Count == 0)
                        {
                            // Not an Intent module (e.g. a shared non-module folder like "Modules/.claude")
                            // - nothing to verify a version against.
                            continue;
                        }

                        var imodspecRelativePaths = imodspecFiles
                            .Select(spec => Normalize(Path.GetRelativePath(repoRoot, spec)))
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var hasNonImodspecChange = normalizedChanged.Any(f =>
                            IsUnderModule(f, moduleName) && !imodspecRelativePaths.Contains(f));

                        if (!hasNonImodspecChange)
                        {
                            continue;
                        }

                        var imodspecChanged = imodspecRelativePaths.Overlaps(normalizedChanged);
                        if (!imodspecChanged)
                        {
                            violations.Add(new ModuleVersionViolation(moduleName, imodspecRelativePaths.First()));
                        }
                    }

                    return new ModuleVersionAuditResult(violations.Count == 0, violations);
                }

                private static bool IsUnderModule(string normalizedRelativePath, string moduleName)
                {
                    var prefix = $"Modules/{moduleName}/";
                    return normalizedRelativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
                }

                private static string? TryGetModuleName(string normalizedRelativePath)
                {
                    const string prefix = "Modules/";
                    if (!normalizedRelativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    var remainder = normalizedRelativePath[prefix.Length..];
                    var slashIndex = remainder.IndexOf('/');
                    if (slashIndex <= 0)
                    {
                        // A file directly under "Modules/" (not inside a module subfolder) is not
                        // itself a module.
                        return null;
                    }

                    return remainder[..slashIndex];
                }

                private static string Normalize(string path) => path.Replace('\\', '/');
            }
            """;

        private const string CloseOutAuditorContent = """
            using System.Xml.Linq;

            namespace Intent.Agent.Gate;

            public sealed record CloseOutFinding(string ModuleName, string Message);

            /// <summary>
            /// Close-out warn checks beyond "did the version move" (see ModuleVersionAuditor for that
            /// one). Each of these turns on a judgment a script cannot make reliably - a docs folder
            /// that genuinely doesn't need updating this time, a tag list that's deliberately short -
            /// so every one of them warns rather than denies; see the module's plan document for why.
            /// </summary>
            public static class CloseOutAuditor
            {
                public static IReadOnlyList<CloseOutFinding> Audit(string repoRoot, IReadOnlyList<string> changedFiles)
                {
                    var normalizedChanged = changedFiles.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var findings = new List<CloseOutFinding>();
                    var modulesDir = Path.Combine(repoRoot, "Modules");

                    foreach (var moduleName in TouchedModules(normalizedChanged))
                    {
                        var moduleDir = Path.Combine(modulesDir, moduleName);
                        if (!Directory.Exists(moduleDir))
                        {
                            continue;
                        }

                        var imodspecPath = Directory.EnumerateFiles(moduleDir, "*.imodspec", SearchOption.AllDirectories).FirstOrDefault();
                        if (imodspecPath is null)
                        {
                            // Not an Intent module - nothing here to check.
                            continue;
                        }

                        var imodspecContent = TryRead(imodspecPath);

                        CheckTags(moduleName, imodspecContent, findings);
                        CheckReadme(moduleName, moduleDir, findings);
                        CheckContext(moduleName, moduleDir, repoRoot, normalizedChanged, findings);
                        CheckReleaseNotesHeading(moduleName, moduleDir, imodspecContent, findings);
                    }

                    return findings;
                }

                private static IEnumerable<string> TouchedModules(IReadOnlySet<string> normalizedChanged)
                {
                    var modules = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var file in normalizedChanged)
                    {
                        const string prefix = "Modules/";
                        if (!file.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var remainder = file[prefix.Length..];
                        var slashIndex = remainder.IndexOf('/');
                        if (slashIndex > 0)
                        {
                            modules.Add(remainder[..slashIndex]);
                        }
                    }

                    return modules;
                }

                private static void CheckTags(string moduleName, string? imodspecContent, List<CloseOutFinding> findings)
                {
                    if (imodspecContent is null)
                    {
                        return;
                    }

                    string? tags;
                    try
                    {
                        tags = (string?)XDocument.Parse(imodspecContent).Root?.Element("tags");
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(tags))
                    {
                        return;
                    }

                    var badTags = tags.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Where(tag => tag != tag.ToLowerInvariant())
                        .ToList();

                    if (badTags.Count > 0)
                    {
                        findings.Add(new CloseOutFinding(moduleName,
                            $"Tags '{string.Join(", ", badTags)}' are not lowercase - see module-docs-chore's tag format guidance."));
                    }
                }

                private static void CheckReadme(string moduleName, string moduleDir, List<CloseOutFinding> findings)
                {
                    if (!File.Exists(Path.Combine(moduleDir, "docs", "README.md")))
                    {
                        findings.Add(new CloseOutFinding(moduleName, "No docs/README.md - see module-docs-chore."));
                    }
                }

                private static void CheckContext(string moduleName, string moduleDir, string repoRoot, IReadOnlySet<string> normalizedChanged, List<CloseOutFinding> findings)
                {
                    var contextPath = Path.Combine(moduleDir, "CONTEXT.md");
                    if (!File.Exists(contextPath))
                    {
                        // Absence is a deliberate choice per module-context-capture, not a finding here.
                        return;
                    }

                    var relativeContext = Normalize(Path.GetRelativePath(repoRoot, contextPath));
                    if (!normalizedChanged.Contains(relativeContext))
                    {
                        findings.Add(new CloseOutFinding(moduleName,
                            "CONTEXT.md was not touched alongside this change - see module-context-capture. " +
                            "If there is genuinely nothing durable to record, that is fine; say so rather than inventing an entry."));
                    }
                }

                private static void CheckReleaseNotesHeading(string moduleName, string moduleDir, string? imodspecContent, List<CloseOutFinding> findings)
                {
                    var releaseNotesPath = Path.Combine(moduleDir, "release-notes.md");
                    if (!File.Exists(releaseNotesPath) || imodspecContent is null)
                    {
                        return;
                    }

                    string? version;
                    try
                    {
                        version = (string?)XDocument.Parse(imodspecContent).Root?.Element("version");
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(version) || !version.Contains("-pre", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    var plainVersion = version[..version.IndexOf("-pre", StringComparison.OrdinalIgnoreCase)];
                    string releaseNotes;
                    try
                    {
                        releaseNotes = File.ReadAllText(releaseNotesPath);
                    }
                    catch (IOException)
                    {
                        return;
                    }

                    // Checking only "does the full -pre string appear" is deliberate: a naive
                    // "and the plain heading is absent" second condition is always false here,
                    // because a version like "1.0.3-pre.0" textually contains its own plain form
                    // "1.0.3" as a substring - "Version 1.0.3-pre.0" already "contains"
                    // "Version 1.0.3". Found by a failing test, not by inspection.
                    if (releaseNotes.Contains($"Version {version}", StringComparison.OrdinalIgnoreCase))
                    {
                        findings.Add(new CloseOutFinding(moduleName,
                            $"release-notes.md heading reads 'Version {version}' - the -pre suffix belongs stripped in the " +
                            $"heading ('Version {plainVersion}'); see module-docs-chore."));
                    }
                }

                private static string? TryRead(string path)
                {
                    try
                    {
                        return File.ReadAllText(path);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                }

                private static string Normalize(string path) => path.Replace('\\', '/');
            }
            """;

        private const string GuardVersionSupportContent = """
            using System.Text.Json;
            using System.Text.RegularExpressions;
            using System.Xml.Linq;

            namespace Intent.Agent.Gate;

            public sealed record DesignerScriptInvocation(string? ApplicationId, string? Script);

            /// <summary>
            /// Extracts the applicationId and script text from a "run_designer_script"-shaped
            /// PreToolUse payload on stdin (tool_input.applicationId / tool_input.script). Tolerant of
            /// the payload not matching - a call to a different MCP tool, or a harness whose payload
            /// shape isn't this one, is simply not a version-setting call.
            /// </summary>
            public static class DesignerScriptInputExtractor
            {
                public static DesignerScriptInvocation Extract(string input)
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return new DesignerScriptInvocation(null, null);
                    }

                    try
                    {
                        using var document = JsonDocument.Parse(input);
                        var root = document.RootElement;

                        if (root.ValueKind == JsonValueKind.Object &&
                            root.TryGetProperty("tool_input", out var toolInput) &&
                            toolInput.ValueKind == JsonValueKind.Object)
                        {
                            return new DesignerScriptInvocation(
                                TryGetString(toolInput, "applicationId"),
                                TryGetString(toolInput, "script"));
                        }

                        return new DesignerScriptInvocation(null, null);
                    }
                    catch (JsonException)
                    {
                        return new DesignerScriptInvocation(null, null);
                    }
                }

                private static string? TryGetString(JsonElement element, string propertyName) =>
                    element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
                        ? property.GetString()
                        : null;
            }

            /// <summary>
            /// Pulls the version string out of a designer script's ".setProperty("Version", "...")"
            /// call. The only scriptable way to set a module's version (confirmed against the live
            /// Module Builder designer script API) is
            /// "pkg.ensureStereotype("Module Settings").setProperty("Version", "X")", so a script that
            /// doesn't match this pattern isn't a version change at all.
            /// </summary>
            public static class VersionScriptExtractor
            {
                private static readonly Regex VersionSetPattern = new(
                    "setProperty\\(\\s*[\"']Version[\"']\\s*,\\s*[\"']([^\"']+)[\"']\\s*\\)",
                    RegexOptions.Compiled);

                public static string? ExtractNewVersion(string script)
                {
                    var match = VersionSetPattern.Match(script);
                    return match.Success ? match.Groups[1].Value : null;
                }
            }

            /// <summary>
            /// Extracts the edit payload from a Write/Edit-style tool_input: the whole new file
            /// content (Write), or the new_string/old_string pair of a partial replacement (Edit).
            /// Tolerant of neither being present - a tool this gate doesn't recognise just gets no
            /// opinion on the edit's content, only its path.
            /// </summary>
            public sealed record EditPayload(string? WholeFileContent, string? EditSnippet);

            public static class StdinEditExtractor
            {
                public static EditPayload Extract(string input)
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return new EditPayload(null, null);
                    }

                    try
                    {
                        using var document = JsonDocument.Parse(input);
                        var root = document.RootElement;

                        if (root.ValueKind != JsonValueKind.Object ||
                            !root.TryGetProperty("tool_input", out var toolInput) ||
                            toolInput.ValueKind != JsonValueKind.Object)
                        {
                            return new EditPayload(null, null);
                        }

                        var wholeFile = TryGetString(toolInput, "content");
                        // "new_string" (Claude Code / Codex convention) and "newString" (OpenCode's
                        // own edit tool - confirmed by probing it directly) are the same concept
                        // under two different naming conventions.
                        var newString = TryGetString(toolInput, "new_string") ?? TryGetString(toolInput, "newString");
                        return new EditPayload(wholeFile, newString);
                    }
                    catch (JsonException)
                    {
                        return new EditPayload(null, null);
                    }
                }

                private static string? TryGetString(JsonElement element, string propertyName) =>
                    element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
                        ? property.GetString()
                        : null;
            }

            /// <summary>
            /// The ".imodspec" is mostly Software-Factory-owned, but three edits are legitimate and
            /// must stay possible: a version downgrade (the SF won't write one from the designer),
            /// interop/dependency entries, and "&lt;tags&gt;". Everything else - notably "&lt;summary&gt;"
            /// and "&lt;description&gt;" - is overwritten by the SF from Application Settings on every
            /// run, discarding a manual edit without an error, so a hand-edit there is always a deny.
            /// </summary>
            public static class ImodspecFieldGuard
            {
                private static readonly string[] DeniedFields = ["summary", "description"];

                /// <param name="currentDiskContent">
                /// The file's current content, when known - present for a whole-file Write (so this
                /// can diff old vs new element-by-element, the only way to catch a full-file rewrite
                /// that changes "&lt;summary&gt;" alongside an innocent "&lt;tags&gt;" edit) and null for a
                /// partial Edit, where only the changed snippet itself is available to inspect.
                /// </param>
                public static bool IsHandEditAllowed(string? currentDiskContent, string editedText)
                {
                    if (currentDiskContent is not null)
                    {
                        foreach (var field in DeniedFields)
                        {
                            if (!string.Equals(ExtractElement(currentDiskContent, field), ExtractElement(editedText, field), StringComparison.Ordinal))
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    foreach (var field in DeniedFields)
                    {
                        if (editedText.Contains($"<{field}>", StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                private static string? ExtractElement(string xml, string elementName)
                {
                    try
                    {
                        return (string?)XDocument.Parse(xml).Root?.Element(elementName);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// Resolves an applicationId to the module folder it lives in, and reads that module's
            /// version, by scanning "Modules/*/*.application.config" for a matching root "id" attribute.
            /// </summary>
            public static class ModuleResolver
            {
                public static string? FindModuleFolderByApplicationId(string repoRoot, string applicationId)
                {
                    var modulesDir = Path.Combine(repoRoot, "Modules");
                    if (!Directory.Exists(modulesDir))
                    {
                        return null;
                    }

                    foreach (var configPath in Directory.EnumerateFiles(modulesDir, "*.application.config", SearchOption.AllDirectories))
                    {
                        string content;
                        try
                        {
                            content = File.ReadAllText(configPath);
                        }
                        catch (IOException)
                        {
                            continue;
                        }

                        if (!content.Contains(applicationId, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        try
                        {
                            var document = XDocument.Load(configPath);
                            var idAttribute = (string?)document.Root?.Attribute("id");
                            if (string.Equals(idAttribute, applicationId, StringComparison.OrdinalIgnoreCase))
                            {
                                return Path.GetDirectoryName(configPath);
                            }
                        }
                        catch (Exception)
                        {
                            // A malformed application.config should not crash the gate - keep looking.
                        }
                    }

                    return null;
                }

                public static string? FindImodspec(string moduleFolder) =>
                    Directory.EnumerateFiles(moduleFolder, "*.imodspec", SearchOption.TopDirectoryOnly).FirstOrDefault();

                public static string? ReadVersionFromImodspecFile(string imodspecPath)
                {
                    try
                    {
                        return ReadVersionFromImodspecContent(File.ReadAllText(imodspecPath));
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                }

                public static string? ReadVersionFromImodspecContent(string content)
                {
                    try
                    {
                        return (string?)XDocument.Parse(content).Root?.Element("version");
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            """;

        private const string CliContent = """
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

                    // Intent's OWN metadata - the designer model, an application's configuration, and
                    // the record of what it generated and installed. This is the whole of what
                    // guard-write protects. Generated OUTPUT is deliberately NOT covered: hand-editing
                    // it is frequently the intended workflow - authoring a scaffolded template, writing
                    // release notes, correcting a csproj package version - and blocking it produced
                    // false positives and nothing else in real use.
                    var metadataViolation = IntentMetadataGuard.DescribeViolation(path);
                    if (metadataViolation is not null)
                    {
                        return Deny(harness, stdout, stderr, metadataViolation);
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

                    // Everything else, including every file the Software Factory generates, is allowed
                    // and stays completely silent - zero tokens for the overwhelming common case.
                    return Allow(harness, stdout);
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
            """;
    }
}
