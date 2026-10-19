using System.Text.Json;

namespace AgentEvals;

/// <summary>
/// One gate invocation, as the gate itself logged it (INTENT_GATE_LOG). <see cref="Harness"/> is the
/// copy's own "--harness" value - which harness's hook file ran it, not necessarily which harness was
/// running: Copilot CLI also runs the hooks in .claude/settings.json.
/// </summary>
public sealed record GateRun(string Command, int ExitCode, int DurationMs, string Stdin, string Stdout, string Stderr, string Harness = "");

/// <summary>One call the agent made to the Intent MCP stub, as the stub logged it, with the files it wrote.</summary>
public sealed record StubCall(string Tool, string Arguments, string Result, bool IsError, IReadOnlyList<StubWrite> Wrote)
{
    /// <summary>The designer script, decoded - the logged arguments are JSON, with quotes escaped.</summary>
    public string Script
    {
        get
        {
            try
            {
                using var arguments = JsonDocument.Parse(Arguments);
                return arguments.RootElement.TryGetProperty("script", out var script) ? script.GetString() ?? "" : "";
            }
            catch (JsonException)
            {
                return "";
            }
        }
    }
}

/// <summary>A file the stub wrote on the designer's behalf, and its content right after.</summary>
public sealed record StubWrite(string Path, string Content);

/// <summary>
/// Everything known about one run. Checks lean on what is on disk, in the gate's own log and in the
/// stub's log, not on what the agent says it did.
/// </summary>
public sealed record Evidence(
    IHarness Harness,
    string WorkspaceRoot,
    ProcessResult Process,
    Transcript Transcript,
    IReadOnlyList<GateRun> GateRuns,
    IReadOnlyList<StubCall> StubCalls,
    IReadOnlyList<string> ChangedFiles,
    string Diff)
{
    public bool Changed(string relativePath) =>
        ChangedFiles.Any(file => string.Equals(file, relativePath, StringComparison.OrdinalIgnoreCase));

    public bool ChangedUnder(string relativeFolder) =>
        ChangedFiles.Any(file => file.StartsWith(relativeFolder.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase));

    public string Read(string relativePath)
    {
        var path = Path.Combine(WorkspaceRoot, relativePath);
        return File.Exists(path) ? File.ReadAllText(path) : "";
    }

    public bool ShellTouched(string fragment) =>
        Transcript.ShellCommands.Any(command => command.Contains(fragment, StringComparison.OrdinalIgnoreCase));

    /// <summary>Denials the gate issued for <paramref name="command"/>, e.g. "guard-write".</summary>
    public IReadOnlyList<GateRun> Denials(string command) =>
        GateRuns.Where(r => r.Command == command && r.ExitCode == 2).ToList();

    /// <summary>
    /// Files under <paramref name="relativeFolder"/> that changed by some route other than the designer:
    /// changed on disk, and not left exactly as the stub last wrote them.
    /// </summary>
    public IReadOnlyList<string> ChangedOutsideDesigner(string relativeFolder) =>
        ChangedFiles
            .Where(file => file.StartsWith(relativeFolder.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase))
            .Where(file => StubCalls.SelectMany(c => c.Wrote).LastOrDefault(w => string.Equals(w.Path, file, StringComparison.OrdinalIgnoreCase)) is not { } last
                || last.Content != Read(file))
            .ToList();

    /// <summary>run_designer_script calls the agent made through the stub.</summary>
    public IReadOnlyList<StubCall> DesignerScripts => StubCalls.Where(c => c.Tool == "run_designer_script").ToList();

    /// <summary>
    /// The index of the first tool call that loaded a skill: the harness's own skill tool, or a read
    /// of its SKILL.md by a read tool or a shell command (how Codex loads one). -1 when never loaded.
    /// </summary>
    public int SkillLoadedAt(string skill) =>
        IndexOf(call => call.Kind == ToolKind.Skill && call.Detail.Equals(skill, StringComparison.OrdinalIgnoreCase)
            || call.Kind is ToolKind.Read or ToolKind.Shell && Normalize(call.Detail).Contains($"{skill}/SKILL.md", StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<string> SkillsLoaded(IEnumerable<string> skills) =>
        skills.Where(skill => SkillLoadedAt(skill) >= 0).ToList();

    /// <summary>The index of the first tool call that read a file whose path contains <paramref name="fragment"/>, or -1.</summary>
    public int FileReadAt(string fragment) =>
        IndexOf(call => call.Kind is ToolKind.Read or ToolKind.Shell && Normalize(call.Detail).Contains(fragment, StringComparison.OrdinalIgnoreCase));

    /// <summary>The index of the first write tool call, or -1.</summary>
    public int FirstWriteAt => IndexOf(call => call.Kind == ToolKind.Write);

    /// <summary>
    /// Whether the agent used any tool at all. Some harnesses leave a denied call out of the transcript,
    /// so the gate log counts too.
    /// </summary>
    public bool UsedTools => Transcript.ToolCalls.Count > 0 || GateRuns.Any(r => r.Command.StartsWith("guard-", StringComparison.Ordinal));

    private int IndexOf(Func<ToolCall, bool> predicate)
    {
        for (var i = 0; i < Transcript.ToolCalls.Count; i++)
        {
            if (predicate(Transcript.ToolCalls[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private static string Normalize(string path) => path.Replace('\\', '/');

    public static IReadOnlyList<GateRun> ReadTelemetry(string path) =>
        ReadJsonLines(path, root => new GateRun(
            GuardFor(root.GetProperty("command").GetString() ?? "", root.GetProperty("stdin").GetString() ?? ""),
            root.GetProperty("exitCode").GetInt32(),
            root.GetProperty("durationMs").GetInt32(),
            root.GetProperty("stdin").GetString() ?? "",
            root.GetProperty("stdout").GetString() ?? "",
            root.GetProperty("stderr").GetString() ?? "",
            HarnessArgument(root)));

    /// <summary>
    /// "guard-tool" (Copilot's single pre-tool hook) dispatches by tool, so a run is counted as the
    /// guard it dispatched to - checks then read the same for every harness.
    /// </summary>
    private static string GuardFor(string command, string stdin) =>
        command != "guard-tool" ? command
        : stdin.Contains("run_designer_script", StringComparison.OrdinalIgnoreCase) ? "guard-version"
        : "guard-write";

    private static string HarnessArgument(JsonElement root)
    {
        if (!root.TryGetProperty("args", out var args) || args.ValueKind != JsonValueKind.Array)
        {
            return "";
        }

        var values = args.EnumerateArray().Select(a => a.GetString() ?? "").ToList();
        var index = values.IndexOf("--harness");
        return index >= 0 && index + 1 < values.Count ? values[index + 1] : "";
    }

    public static IReadOnlyList<StubCall> ReadStubLog(string path) =>
        ReadJsonLines(path, root => new StubCall(
            root.GetProperty("tool").GetString() ?? "",
            root.GetProperty("arguments").GetRawText(),
            root.GetProperty("result").GetString() ?? "",
            root.GetProperty("isError").GetBoolean(),
            root.TryGetProperty("wrote", out var wrote) && wrote.ValueKind == JsonValueKind.Array
                ? wrote.EnumerateArray().Select(w => new StubWrite(w.GetProperty("path").GetString() ?? "", w.GetProperty("content").GetString() ?? "")).ToList()
                : []));

    private static IReadOnlyList<T> ReadJsonLines<T>(string path, Func<JsonElement, T> read)
    {
        if (!File.Exists(path))
        {
            return [];
        }

        var items = new List<T>();
        foreach (var line in File.ReadAllLines(path).Where(l => l.TrimStart().StartsWith('{')))
        {
            try
            {
                items.Add(read(JsonDocument.Parse(line).RootElement));
            }
            catch (Exception)
            {
                // A half-written line from a killed run is not worth failing the report over.
            }
        }

        return items;
    }
}
