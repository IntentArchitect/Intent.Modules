using System.Text.Json;

namespace AgentEvals;

/// <summary>
/// Runs the requested layers - wiring checks, then each requested harness through each requested
/// scenario, one at a time - and writes a report into a fresh run folder under the eval root.
/// </summary>
public static class Conductor
{
    public const string Usage = """
        Usage: dotnet run evals.cs -- [options]

          --layer wiring,hooks,guidance   layers to run (default: all)
          --harness claude,codex,...      harnesses to run (default: all ready ones)
          --scenario S1,G3,...            scenarios to run (default: every scenario in the chosen layers)
          --prompt "..."                  run one free-form prompt instead of scenarios (full evidence, no checks)
          --no-stub                       with --prompt: do not attach the Intent MCP stub
          --repeat N                      run each scenario N times and report a pass rate (default 1)
          --claude-model M                Claude Code's model (default haiku)
          --codex-model M                 Codex's model (default gpt-5.4-mini - OpenAI, on the API key)
          --opencode-model M              OpenCode's model (default openrouter/z-ai/glm-5.2)
          --copilot-model M               Copilot CLI's model (default gpt-5-mini)
          --judge-model M                 the judge's model (default sonnet)
          --no-judge                      skip the judge
          --timeout S                     seconds before a run is stopped (default 300)
          --root PATH                     the eval root (default %LOCALAPPDATA%\IntentAgentEvals, or INTENT_AGENT_EVALS_ROOT)
          --keep-runs N                   run folders to keep, newest first (default 10)
          --list                          show harnesses, scenarios and the eval root, then stop
        """;

    public static async Task<int> RunAsync(string[] args)
    {
        Options options;
        try
        {
            options = Options.Parse(args);
        }
        catch (ArgumentException error)
        {
            Console.Error.WriteLine(error.Message);
            Console.Error.WriteLine();
            Console.Error.WriteLine(Usage);
            return 2;
        }

        var paths = EvalPaths.Locate(options.Root);
        paths.UseOwnTemp();
        var harnesses = Harnesses.Create(paths);

        if (Validate(options, harnesses) is { } invalid)
        {
            Console.Error.WriteLine(invalid);
            return 2;
        }

        if (options.List)
        {
            await ListAsync(paths, harnesses);
            return 0;
        }

        // Keep room for the run about to start.
        foreach (var pruned in paths.PruneRuns(options.KeepRuns - 1))
        {
            Console.WriteLine($"[pruned] {pruned}");
        }

        if (paths.PruneBuildCache() is > 0 and var cached)
        {
            Console.WriteLine($"[pruned] {cached} cached gate build(s) of workspaces that no longer exist");
        }

        var startedAt = DateTime.Now;
        var runDirectory = Path.Combine(paths.Runs, $"{startedAt:yyyyMMdd-HHmmss}-{(OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "macos" : "linux")}");
        Directory.CreateDirectory(runDirectory);
        var (commit, dirty) = await InputsCommitAsync(paths);
        var info = new RunInfo(startedAt, commit, dirty, args, [], []);
        var results = new List<RunResult>();
        Console.WriteLine($"Run folder: {runDirectory}");

        if (options.Prompt is null && options.RunsLayer("wiring"))
        {
            Console.Write("[wiring] ");
            var wiring = await Wiring.CheckAsync(paths, options.Harnesses, runDirectory);
            results.AddRange(wiring);
            Console.WriteLine(string.Join(", ", wiring.Select(w => $"{w.Harness} {w.Checks.Count(c => c.Passed)}/{w.Checks.Count}")));
        }

        var scenarios = options.Prompt is not null
            ? [new AdHocPrompt(options.Prompt, options.WithStub)]
            : Scenarios.All.Where(s => options.Scenarios is not null ? options.Scenarios.Contains(s.Id) : s.Layer != Layer.AdHoc && options.RunsLayer(s.Layer.ToString().ToLowerInvariant())).ToList();

        if (scenarios.Count > 0)
        {
            // Harnesses share nothing at run time - own workspaces, stubs and homes - so they run side by
            // side; each harness still runs its own scenarios one at a time.
            var selected = harnesses.Where(h => options.Harnesses is null || options.Harnesses.Contains(h.Id)).ToList();
            var containment = await Containment.CaptureAsync(paths);
            var perHarness = await Task.WhenAll(selected.Select(harness => RunHarnessAsync(paths, harness, scenarios, options, runDirectory, info, containment)));
            results.AddRange(perHarness.SelectMany(r => r));
        }

        File.WriteAllText(Path.Combine(runDirectory, "run.json"), JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
        var summary = Report.Write(runDirectory, info, results);
        Console.WriteLine();
        Console.WriteLine($"Report: {summary}");
        return 0;
    }

    /// <summary>Runs every scenario for one harness, unless it is not ready; stops early after a sign-in failure.</summary>
    private static async Task<List<RunResult>> RunHarnessAsync(EvalPaths paths, IHarness harness, IReadOnlyList<Scenario> scenarios, Options options, string runDirectory, RunInfo info, Containment containment)
    {
        var results = new List<RunResult>();
        string version;
        try
        {
            if (await harness.NotReadyAsync() is { } reason)
            {
                Skip(info, harness, reason);
                return results;
            }

            version = await harness.VersionAsync();
        }
        catch (Exception error)
        {
            Skip(info, harness, $"could not be started: {error.Message}");
            return results;
        }

        lock (info)
        {
            info.HarnessVersions[harness.Id] = version;
        }

        foreach (var scenario in scenarios)
        {
            for (var attempt = 1; attempt <= options.Repeat; attempt++)
            {
                var label = $"[{harness.Id} {scenario.Id}{(options.Repeat > 1 ? $" #{attempt}" : "")}] {scenario.Title}";
                var folder = $"{harness.Id}-{scenario.Id}" + (options.Repeat > 1 ? $"-r{attempt}" : "");
                if (containment.Breached)
                {
                    Skip(info, harness, "stopped: a run changed files outside its workspace - see the Contained check");
                    return results;
                }

                var (result, signInFailed) = await RunOneAsync(paths, harness, version, scenario, options, Path.Combine(runDirectory, folder), attempt);
                var escaped = await containment.ChangedSinceBaselineAsync();
                result = result with
                {
                    Checks = [.. result.Checks, new Check("Contained", escaped.Count == 0, escaped.Count == 0
                        ? "nothing outside the workspace changed"
                        : $"files in the repository changed while this run was in progress: {string.Join(", ", escaped.Take(10))}")],
                };
                if (Directory.Exists(Path.Combine(runDirectory, folder)))
                {
                    File.WriteAllText(Path.Combine(runDirectory, folder, "checks.json"), JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
                }

                results.Add(result);
                Console.WriteLine($"{label} ... " + (result.Error is null ? $"{(result.Passed ? "pass" : "FAIL")} {result.Rating} ({result.Seconds:0}s)" : $"error: {result.Error}"));

                // Every later run of this harness would fail the same way, after the same retries.
                if (signInFailed)
                {
                    Skip(info, harness, "stopped after a sign-in failure; its remaining scenarios were not run");
                    return results;
                }
            }
        }

        return results;
    }

    private static void Skip(RunInfo info, IHarness harness, string reason)
    {
        lock (info)
        {
            info.Skipped[harness.Id] = reason;
        }

        Console.WriteLine($"[skip] {harness.DisplayName}: {reason}");
    }

    private static async Task<(RunResult Result, bool SignInFailed)> RunOneAsync(
        EvalPaths paths, IHarness harness, string version, Scenario scenario, Options options, string runDirectory, int attempt)
    {
        RunResult Failed(string error, double seconds = 0) =>
            new(harness.Id, version, scenario.Id, scenario.Title, [], [], 0, 0, seconds, error, scenario.Layer.ToString(), attempt);

        try
        {
            var workspace = await Workspace.CreateAsync(paths, harness, scenario, runDirectory);
            var startInfo = harness.CreateRun(new RunRequest(workspace, scenario.Prompt, options.ModelFor(harness.Id), scenario.UsesIntentStub));
            startInfo.Environment["INTENT_GATE_LOG"] = workspace.TelemetryPath;
            scenario.AdjustEnvironment(startInfo.Environment);
            var process = await Processes.RunAsync(startInfo, stdin: null, options.Timeout);

            File.WriteAllText(Path.Combine(runDirectory, "transcript.jsonl"), process.Stdout);
            File.WriteAllText(Path.Combine(runDirectory, "stderr.txt"), process.Stderr);

            if (harness.SignInFailure(process) is { } signInFailure)
            {
                return (Failed(signInFailure, process.Duration.TotalSeconds), true);
            }

            var transcript = harness.ParseTranscript(process.Stdout);
            var changed = await workspace.ChangedFilesAsync();
            var diff = await workspace.DiffAsync();
            File.WriteAllText(Path.Combine(runDirectory, "diff.patch"), diff);

            var evidence = new Evidence(harness, workspace.Root, process, transcript,
                Evidence.ReadTelemetry(workspace.TelemetryPath), Evidence.ReadStubLog(workspace.StubLogPath), changed, diff);
            var checks = scenario.Checks(evidence).ToList();
            var judged = options.Judge ? await Judge.RateAsync(scenario, evidence, options.JudgeModel, paths.JudgeDirectory) : [];

            return (new RunResult(harness.Id, version, scenario.Id, scenario.Title, checks, judged,
                transcript.Turns, evidence.GateRuns.Count, process.Duration.TotalSeconds, null, scenario.Layer.ToString(), attempt), false);
        }
        catch (Exception exception)
        {
            return (Failed(exception.Message), false);
        }
    }

    private static async Task ListAsync(EvalPaths paths, IReadOnlyList<IHarness> harnesses)
    {
        Console.WriteLine($"Eval root: {paths.Root}");
        Console.WriteLine($"Inputs:    {paths.Fixture}");
        Console.WriteLine($"           {paths.GeneratedOutput}");
        Console.WriteLine();
        Console.WriteLine("Harnesses (run):");
        foreach (var harness in harnesses)
        {
            Console.WriteLine($"  {harness.Id,-9} {harness.DisplayName,-12} {await harness.NotReadyAsync() ?? harness.Executable}");
        }

        Console.WriteLine($"Harnesses (wiring only): {string.Join(", ", Wiring.Ids.Except(harnesses.Select(h => h.Id)))}");
        Console.WriteLine();
        Console.WriteLine("Scenarios:");
        foreach (var scenario in Scenarios.All)
        {
            Console.WriteLine($"  {scenario.Id,-4} {scenario.Layer,-9} {scenario.Title}");
        }
    }

    private static string? Validate(Options options, IReadOnlyList<IHarness> harnesses)
    {
        var knownHarnesses = harnesses.Select(h => h.Id).Concat(Wiring.Ids).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var unknownHarnesses = options.Harnesses?.Where(h => !knownHarnesses.Contains(h)).ToList() ?? [];
        if (unknownHarnesses.Count > 0)
        {
            return $"Unknown harness(es): {string.Join(", ", unknownHarnesses)}. Known: {string.Join(", ", knownHarnesses.Order())}.";
        }

        var knownScenarios = Scenarios.All.Select(s => s.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var unknownScenarios = options.Scenarios?.Where(s => !knownScenarios.Contains(s)).ToList() ?? [];
        if (unknownScenarios.Count > 0)
        {
            return $"Unknown scenario(s): {string.Join(", ", unknownScenarios)}. Known: {string.Join(", ", knownScenarios)}.";
        }

        var unknownLayers = options.Layers?.Where(l => l is not ("wiring" or "hooks" or "guidance")).ToList() ?? [];
        if (unknownLayers.Count > 0)
        {
            return $"Unknown layer(s): {string.Join(", ", unknownLayers)}. Known: wiring, hooks, guidance.";
        }

        if (options.Prompt is not null && (options.Scenarios is not null || options.Layers is not null))
        {
            return "--prompt runs one free-form prompt; it cannot be combined with --scenario or --layer.";
        }

        return null;
    }

    private static async Task<(string Commit, bool Dirty)> InputsCommitAsync(EvalPaths paths)
    {
        var head = await Processes.RunAsync(Processes.Command("git", paths.RepoRoot, "rev-parse", "--short", "HEAD"), null, TimeSpan.FromSeconds(30));
        var status = await Processes.RunAsync(
            Processes.Command("git", paths.RepoRoot, "status", "--porcelain", "--", paths.Fixture, paths.GeneratedOutput, paths.SuiteDirectory),
            null, TimeSpan.FromSeconds(30));
        return (head.Stdout.Trim(), status.Stdout.Trim().Length > 0);
    }

    private sealed record Options(
        HashSet<string>? Harnesses,
        HashSet<string>? Scenarios,
        HashSet<string>? Layers,
        string? Prompt,
        bool WithStub,
        int Repeat,
        IReadOnlyDictionary<string, string> Models,
        string JudgeModel,
        bool Judge,
        TimeSpan Timeout,
        string? Root,
        int KeepRuns,
        bool List)
    {
        private static readonly string[] ValueOptions =
            ["--harness", "--scenario", "--layer", "--prompt", "--repeat", "--claude-model", "--codex-model", "--opencode-model", "--copilot-model", "--judge-model", "--timeout", "--root", "--keep-runs"];

        private static readonly string[] Flags = ["--no-judge", "--no-stub", "--list"];

        /// <summary>The model to run a harness on, or null for that harness adapter's own default.</summary>
        public string? ModelFor(string harnessId) => Models.TryGetValue(harnessId, out var model) ? model : null;

        /// <summary>Whether a layer runs: named in --layer, or every layer when none is named.</summary>
        public bool RunsLayer(string name) => Layers is null || Layers.Contains(name);

        public static Options Parse(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (ValueOptions.Contains(args[i]))
                {
                    if (i + 1 >= args.Length)
                    {
                        throw new ArgumentException($"{args[i]} needs a value.");
                    }

                    i++;
                }
                else if (!Flags.Contains(args[i]))
                {
                    throw new ArgumentException($"Unknown option: {args[i]}");
                }
            }

            string? Value(string name) => Array.IndexOf(args, name) is var index and >= 0 ? args[index + 1] : null;

            HashSet<string>? Set(string name) =>
                Value(name) is { } value ? value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet(StringComparer.OrdinalIgnoreCase) : null;

            int Int(string name, int fallback) =>
                Value(name) is { } value ? int.TryParse(value, out var number) && number > 0 ? number : throw new ArgumentException($"{name} needs a positive number.") : fallback;

            return new Options(
                Set("--harness"),
                Set("--scenario"),
                Set("--layer"),
                Value("--prompt"),
                !args.Contains("--no-stub"),
                Int("--repeat", 1),
                new[] { "claude", "codex", "opencode", "copilot" }.Where(id => Value($"--{id}-model") is not null).ToDictionary(id => id, id => Value($"--{id}-model")!),
                Value("--judge-model") ?? "sonnet",
                !args.Contains("--no-judge"),
                TimeSpan.FromSeconds(Int("--timeout", 300)),
                Value("--root"),
                Int("--keep-runs", 10),
                args.Contains("--list"));
        }
    }
}
