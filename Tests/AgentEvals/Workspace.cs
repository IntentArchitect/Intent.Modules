namespace AgentEvals;

/// <summary>
/// Where the suite reads its inputs and writes everything else. Inputs - the fixture module and the
/// test application's generated output - live in the repository and are only ever read. Everything the
/// suite writes goes under one root it owns, outside the repository.
/// </summary>
public sealed record EvalPaths(string SuiteDirectory, string RepoRoot, string Root)
{
    /// <summary>The fake module every workspace starts from.</summary>
    public string Fixture => Path.Combine(SuiteDirectory, "Fixture");

    /// <summary>
    /// The test application's generated output: the hook files, gate, instructions and skills the
    /// modules produce today. Regenerate that application before a run to evaluate a module change.
    /// </summary>
    public string GeneratedOutput => Path.Combine(RepoRoot, "Tests", "ModuleBuilderSkills");

    /// <summary>Harness sign-ins and runner-owned harness configuration, kept between runs.</summary>
    public string Homes => Path.Combine(Root, "homes");

    /// <summary>One folder per invocation, pruned to the newest N.</summary>
    public string Runs => Path.Combine(Root, "runs");

    /// <summary>The empty folder the judge runs from, so it can see no project.</summary>
    public string JudgeDirectory => Path.Combine(Root, "judge");

    /// <summary>
    /// The temp folder every process the runner starts is given - harnesses, gates, git, dotnet.
    /// </summary>
    public string Temp => Path.Combine(Root, "tmp");

    /// <summary>
    /// Points TEMP/TMP (TMPDIR elsewhere) at <see cref="Temp"/> for this process, so every child inherits
    /// it. dotnet caches each workspace's gate build under the temp folder, and harnesses write their own
    /// scratch there; keeping both under the root keeps all of a run's churn on the root's drive - which
    /// matters when that drive is the one excluded from antivirus scanning and the system drive is not.
    /// </summary>
    public void UseOwnTemp()
    {
        Directory.CreateDirectory(Temp);
        foreach (var name in OperatingSystem.IsWindows() ? new[] { "TEMP", "TMP" } : ["TMPDIR"])
        {
            Environment.SetEnvironmentVariable(name, Temp);
        }
    }

    public static EvalPaths Locate(string? rootOverride)
    {
        // File-based apps expose their own source folder; fall back to the working directory.
        var start = AppContext.GetData("EntryPointFileDirectoryPath") as string ?? Directory.GetCurrentDirectory();
        var suite = Path.GetFullPath(start);

        var current = new DirectoryInfo(suite);
        while (current is not null && !Directory.Exists(Path.Combine(current.FullName, ".git")) && !File.Exists(Path.Combine(current.FullName, ".git")))
        {
            current = current.Parent;
        }

        if (current is null)
        {
            throw new InvalidOperationException($"Could not find the repository root above '{suite}'.");
        }

        var root = rootOverride
            ?? Environment.GetEnvironmentVariable("INTENT_AGENT_EVALS_ROOT")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IntentAgentEvals");
        root = Path.GetFullPath(root);

        if (IsUnder(root, current.FullName))
        {
            throw new InvalidOperationException($"The eval root '{root}' is inside the repository '{current.FullName}'. Choose a root outside it.");
        }

        return new EvalPaths(suite, current.FullName, root);
    }

    /// <summary>Keeps the newest <paramref name="keep"/> run folders and deletes the rest.</summary>
    public IReadOnlyList<string> PruneRuns(int keep)
    {
        if (!Directory.Exists(Runs))
        {
            return [];
        }

        // Run folders are named yyyyMMdd-HHmmss-os, so name order is age order.
        var stale = Directory.GetDirectories(Runs).OrderByDescending(Path.GetFileName, StringComparer.Ordinal).Skip(Math.Max(keep, 0)).ToList();
        foreach (var folder in stale)
        {
            Workspace.DeleteTree(folder);
        }

        return stale;
    }

    /// <summary>
    /// Removes dotnet's cached builds of gates whose workspace no longer exists. dotnet caches each
    /// file-based app's build by path under %TEMP%/dotnet/runfile - <see cref="Temp"/> once
    /// <see cref="UseOwnTemp"/> has run - so every workspace adds one entry. Only
    /// entries whose recorded project lies under the eval root and is gone are touched; anything else in
    /// the cache belongs to someone else.
    /// </summary>
    public int PruneBuildCache()
    {
        var cache = Path.Combine(Path.GetTempPath(), "dotnet", "runfile");
        if (!Directory.Exists(cache))
        {
            return 0;
        }

        var removed = 0;
        foreach (var folder in Directory.GetDirectories(cache, "gate-*"))
        {
            var assets = Path.Combine(folder, "obj", "project.assets.json");
            try
            {
                using var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(assets));
                var project = json.RootElement.GetProperty("project").GetProperty("restore").GetProperty("projectPath").GetString();
                if (project is not null && IsUnder(Path.GetFullPath(project), Root) && !Directory.Exists(Path.GetDirectoryName(project)))
                {
                    Workspace.DeleteTree(folder);
                    removed++;
                }
            }
            catch (Exception)
            {
                // Unreadable or in use - leave it; the SDK ages its cache out on its own.
            }
        }

        return removed;
    }

    private static bool IsUnder(string path, string folder) =>
        (path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
            .StartsWith(folder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// The git repository one harness works in for one scenario: the fixture module plus the files the
/// modules generate for that harness, committed as a baseline so every change the agent makes shows
/// up in the diff. It lives inside its run's folder and is kept with the evidence, for inspection.
/// </summary>
public sealed class Workspace
{
    private Workspace(string root, string runDirectory)
    {
        Root = root;
        RunDirectory = runDirectory;
    }

    public string Root { get; }

    /// <summary>Where this run's evidence goes - beside the workspace, never inside it.</summary>
    public string RunDirectory { get; }

    /// <summary>The gate's own log. Outside the workspace, so it never appears in the agent's diff or tempts it.</summary>
    public string TelemetryPath => Path.Combine(RunDirectory, "gate-telemetry.jsonl");

    /// <summary>The Intent MCP stub's record of every call the agent made to it.</summary>
    public string StubLogPath => Path.Combine(RunDirectory, "intent-mcp-calls.jsonl");

    public static async Task<Workspace> CreateAsync(EvalPaths paths, IHarness harness, Scenario scenario, string runDirectory)
    {
        var root = Path.Combine(runDirectory, "workspace");
        Directory.CreateDirectory(root);

        CopyDirectory(paths.Fixture, root);
        foreach (var relative in harness.GeneratedFiles.Concat(scenario.AlsoGenerated).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var source = Path.Combine(paths.GeneratedOutput, relative);
            var target = Path.Combine(root, relative);
            if (Directory.Exists(source))
            {
                CopyDirectory(source, target);
            }
            else if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.Copy(source, target, overwrite: true);
            }
            else
            {
                throw new InvalidOperationException(
                    $"'{relative}' is missing from {paths.GeneratedOutput}. Regenerate the test application before running the evals.");
            }
        }

        scenario.Arrange(root);

        await Git(root, "init", "-q");
        await Git(root, "add", "-A");
        await Git(root, "-c", "user.name=agent-evals", "-c", "user.email=agent-evals@localhost", "commit", "-q", "-m", "baseline");

        // Work already in progress when the agent arrives - uncommitted, as it would be mid-change.
        scenario.ArrangeInProgress(root);

        if (scenario.WarmGate)
        {
            // Built up front so the first hook call measures the gate, not a cold compile.
            var warm = await Processes.RunAsync(
                Processes.Command("dotnet", root, "run", harness.GatePath, "--", "warm"), stdin: null, TimeSpan.FromMinutes(3));
            if (warm.ExitCode != 0)
            {
                throw new InvalidOperationException($"Warming the gate failed in {root}:\n{warm.Stderr}{warm.Stdout}");
            }
        }

        return new Workspace(root, runDirectory);
    }

    public async Task<IReadOnlyList<string>> ChangedFilesAsync()
    {
        var status = await Git(Root, "status", "--porcelain", "--untracked-files=all");
        return status.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Length > 3 ? line[3..].Trim().Trim('"').Replace('\\', '/') : line.Trim())
            .ToList();
    }

    public async Task<string> DiffAsync()
    {
        // Staged so new files appear in the diff too; the workspace's index is the runner's to use.
        await Git(Root, "add", "-A");
        return await Git(Root, "diff", "--cached");
    }

    /// <summary>Deletes a folder tree, including git's read-only object files.</summary>
    public static void DeleteTree(string folder)
    {
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
        {
            File.SetAttributes(file, FileAttributes.Normal);
        }

        Directory.Delete(folder, recursive: true);
    }

    private static async Task<string> Git(string root, params string[] args)
    {
        var result = await Processes.RunAsync(Processes.Command("git", root, args), stdin: null, TimeSpan.FromMinutes(1));
        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"git {string.Join(' ', args)} failed in {root}:\n{result.Stderr}");
        }

        return result.Stdout;
    }

    public static void CopyDirectory(string source, string target)
    {
        foreach (var directory in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(Path.Combine(target, Path.GetRelativePath(source, directory)));
        }

        Directory.CreateDirectory(target);
        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, Path.Combine(target, Path.GetRelativePath(source, file)), overwrite: true);
        }
    }
}

/// <summary>
/// Detects an agent changing anything outside its own workspace - in practice, the repository the inputs
/// come from. Agents run unconfined, and a harness that resolves its project folder wrongly will happily
/// edit the real fixture. A fingerprint of the repository (every changed or untracked path, with a hash of
/// its content) is taken before the first run and compared after each one.
/// </summary>
public sealed class Containment
{
    private readonly EvalPaths paths;
    private readonly Dictionary<string, string> baseline;

    private Containment(EvalPaths paths, Dictionary<string, string> baseline)
    {
        this.paths = paths;
        this.baseline = baseline;
    }

    /// <summary>Set once any run changed the repository; every harness stops at its next run.</summary>
    public bool Breached { get; private set; }

    public static async Task<Containment> CaptureAsync(EvalPaths paths) => new(paths, await FingerprintAsync(paths));

    /// <summary>Repository paths that differ from the baseline, or none.</summary>
    public async Task<IReadOnlyList<string>> ChangedSinceBaselineAsync()
    {
        var now = await FingerprintAsync(paths);
        var changed = now.Where(e => !baseline.TryGetValue(e.Key, out var hash) || hash != e.Value).Select(e => e.Key)
            .Concat(baseline.Keys.Where(k => !now.ContainsKey(k)))
            .Distinct()
            .Order()
            .ToList();
        if (changed.Count > 0)
        {
            Breached = true;
        }

        return changed;
    }

    private static async Task<Dictionary<string, string>> FingerprintAsync(EvalPaths paths)
    {
        // --no-optional-locks: runs happen side by side, and a status refresh must not take the index lock.
        var status = await Processes.RunAsync(
            Processes.Command("git", paths.RepoRoot, "--no-optional-locks", "status", "--porcelain=v1", "-z", "--untracked-files=all"),
            null, TimeSpan.FromMinutes(1));
        var fingerprint = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in status.Stdout.Split('\0', StringSplitOptions.RemoveEmptyEntries).Where(e => e.Length > 3))
        {
            var path = entry[3..];
            var full = Path.Combine(paths.RepoRoot, path);
            fingerprint[path] = entry[..2] + ":" + (File.Exists(full)
                ? Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(full)))
                : "gone");
        }

        return fingerprint;
    }
}
