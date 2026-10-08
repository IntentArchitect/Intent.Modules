using System.Text.Json;

namespace AgentEvals;

/// <summary>
/// What a harness's own output says happened, condensed to a common shape. Events are one line each
/// and bounded, so the judge sees the whole run without a raw multi-megabyte log.
/// </summary>
public sealed record Transcript(string FinalMessage, IReadOnlyList<string> Events, int Turns, IReadOnlyList<ToolCall> ToolCalls)
{
    public IEnumerable<string> ShellCommands => ToolCalls.Where(c => c.Kind == ToolKind.Shell).Select(c => c.Detail);
}

public enum ToolKind
{
    Read,
    Search,
    Write,
    Shell,
    Skill,
    Mcp,
    Other,
}

/// <summary>One tool call the agent made, in order. <see cref="Detail"/> is the path, command, skill or arguments.</summary>
public sealed record ToolCall(ToolKind Kind, string Name, string Detail);

public static class TranscriptParsers
{
    /// <summary>Claude Code's "--output-format stream-json --verbose" events.</summary>
    public static Transcript ClaudeStreamJson(string stdout)
    {
        var events = new List<string>();
        var calls = new List<ToolCall>();
        var final = "";
        var turns = 0;

        foreach (var root in JsonLines(stdout))
        {
            var type = Str(root, "type");
            switch (type)
            {
                case "assistant" when root.TryGetProperty("message", out var message):
                    foreach (var part in Items(message, "content"))
                    {
                        switch (Str(part, "type"))
                        {
                            case "text":
                                events.Add("assistant: " + Trunc(Str(part, "text"), 600));
                                break;
                            case "tool_use":
                                var input = part.TryGetProperty("input", out var i) ? i.GetRawText() : "";
                                events.Add($"tool_use {Str(part, "name")}: {Trunc(input, 400)}");
                                calls.Add(ClaudeToolCall(Str(part, "name"), i));
                                break;
                        }
                    }

                    break;

                case "user" when root.TryGetProperty("message", out var message):
                    foreach (var part in Items(message, "content").Where(p => Str(p, "type") == "tool_result"))
                    {
                        var isError = part.TryGetProperty("is_error", out var e) && e.ValueKind == JsonValueKind.True;
                        events.Add($"tool_result{(isError ? " (error)" : "")}: {Trunc(ContentText(part), 600)}");
                    }

                    break;

                case "system":
                    var subtype = Str(root, "subtype");
                    if (subtype.Contains("hook", StringComparison.OrdinalIgnoreCase))
                    {
                        events.Add($"system {subtype}: {Trunc(root.GetRawText(), 400)}");
                    }

                    break;

                case "result":
                    final = Str(root, "result");
                    turns = root.TryGetProperty("num_turns", out var n) && n.TryGetInt32(out var count) ? count : turns;
                    events.Add($"result {Str(root, "subtype")}: {Trunc(final, 600)}");
                    break;
            }
        }

        return new Transcript(final, events, turns, calls);
    }

    private static ToolCall ClaudeToolCall(string name, JsonElement input) => name switch
    {
        "Read" => new(ToolKind.Read, name, Str(input, "file_path")),
        "Glob" or "Grep" or "LS" => new(ToolKind.Search, name, Str(input, "pattern") + " " + Str(input, "path")),
        "Write" or "Edit" or "MultiEdit" or "NotebookEdit" => new(ToolKind.Write, name, Str(input, "file_path")),
        "Bash" or "PowerShell" => new(ToolKind.Shell, name, Str(input, "command")),
        "Skill" => new(ToolKind.Skill, name, Str(input, "skill")),
        _ when name.StartsWith("mcp__", StringComparison.Ordinal) => new(ToolKind.Mcp, name, input.ValueKind == JsonValueKind.Undefined ? "" : input.GetRawText()),
        _ => new(ToolKind.Other, name, input.ValueKind == JsonValueKind.Undefined ? "" : Trunc(input.GetRawText(), 400)),
    };

    /// <summary>Codex's "exec --json" events.</summary>
    public static Transcript CodexJson(string stdout)
    {
        var events = new List<string>();
        var calls = new List<ToolCall>();
        var final = "";
        var turns = 0;

        foreach (var root in JsonLines(stdout))
        {
            var type = Str(root, "type");
            if (type == "turn.completed")
            {
                turns++;
                continue;
            }

            if (type == "error")
            {
                events.Add("error: " + Trunc(Str(root, "message"), 600));
                continue;
            }

            if (type != "item.completed" || !root.TryGetProperty("item", out var item))
            {
                continue;
            }

            switch (Str(item, "type"))
            {
                case "agent_message":
                    final = Str(item, "text");
                    events.Add("assistant: " + Trunc(final, 600));
                    break;
                case "command_execution":
                    calls.Add(new ToolCall(ToolKind.Shell, "shell", Str(item, "command")));
                    events.Add($"shell (exit {Raw(item, "exit_code")}): {Trunc(Str(item, "command"), 300)} -> {Trunc(Str(item, "aggregated_output"), 300)}");
                    break;
                case "file_change":
                    foreach (var change in Items(item, "changes"))
                    {
                        calls.Add(new ToolCall(ToolKind.Write, "apply_patch", Str(change, "path")));
                    }

                    events.Add($"file_change ({Str(item, "status")}): {Trunc(Raw(item, "changes"), 400)}");
                    break;
                case "mcp_tool_call":
                    calls.Add(new ToolCall(ToolKind.Mcp, $"mcp__{Str(item, "server")}__{Str(item, "tool")}", Raw(item, "arguments")));
                    events.Add($"mcp {Str(item, "server")}.{Str(item, "tool")} ({Str(item, "status")}): {Trunc(Raw(item, "arguments"), 300)} -> {Trunc(Raw(item, "result"), 300)}");
                    break;
                case "reasoning":
                    break;
                default:
                    events.Add($"{Str(item, "type")}: {Trunc(item.GetRawText(), 400)}");
                    break;
            }
        }

        return new Transcript(final, events, turns, calls);
    }

    /// <summary>
    /// OpenCode's "run --format json" events: tool_use (part.tool, part.state.input/output), text
    /// (part.text), and step_finish once per model step.
    /// </summary>
    public static Transcript OpenCodeJson(string stdout)
    {
        var events = new List<string>();
        var calls = new List<ToolCall>();
        var final = "";
        var steps = 0;

        foreach (var root in JsonLines(stdout))
        {
            if (!root.TryGetProperty("part", out var part))
            {
                continue;
            }

            switch (Str(root, "type"))
            {
                case "tool_use":
                    var state = part.TryGetProperty("state", out var s) ? s : default;
                    var input = Raw(state, "input");
                    calls.Add(Classify(Str(part, "tool"), input.Length > 0 ? input : "{}"));
                    events.Add($"tool {Str(part, "tool")} ({Str(state, "status")}): {Trunc(input, 400)} -> {Trunc(Str(state, "output") + Str(state, "error"), 300)}");
                    break;
                case "text":
                    final = Str(part, "text");
                    events.Add("assistant: " + Trunc(final, 600));
                    break;
                case "step_finish":
                    steps++;
                    break;
            }
        }

        return new Transcript(final, events, steps, calls);
    }

    /// <summary>
    /// Copilot CLI's "--output-format json" events: tool.execution_start (data.toolName, data.arguments),
    /// tool.execution_complete, assistant.message (data.content) and assistant.turn_end. The many
    /// assistant.*_delta events are streaming fragments of the same content and are skipped.
    /// </summary>
    public static Transcript CopilotJson(string stdout)
    {
        var events = new List<string>();
        var calls = new List<ToolCall>();
        var final = "";
        var turns = 0;

        foreach (var root in JsonLines(stdout))
        {
            var data = root.TryGetProperty("data", out var d) ? d : default;
            switch (Str(root, "type"))
            {
                case "tool.execution_start":
                    var arguments = Raw(data, "arguments");
                    calls.Add(Classify(Str(data, "toolName"), arguments.Length > 0 && arguments.StartsWith('{') ? arguments : "{}"));
                    events.Add($"tool {Str(data, "toolName")}: {Trunc(arguments, 400)}");
                    break;
                case "tool.execution_complete":
                    events.Add($"tool result{(Raw(data, "success") == "false" ? " (error)" : "")}: {Trunc(Raw(data, "result") + Raw(data, "error"), 400)}");
                    break;
                case "assistant.message" when Str(data, "content").Length > 0:
                    final = Str(data, "content");
                    events.Add("assistant: " + Trunc(final, 600));
                    break;
                case "assistant.turn_end":
                    turns++;
                    break;
            }
        }

        return new Transcript(final, events, turns, calls);
    }

    private static ToolCall Classify(string tool, string input)
    {
        var name = tool.ToLowerInvariant();
        using var document = JsonDocument.Parse(input);
        var args = document.RootElement;
        var path = FirstOf(args, "filePath", "file_path", "path", "filename");
        return name switch
        {
            _ when name.Contains("run_designer_script") || name.Contains("get_applications") || name.StartsWith("mcp", StringComparison.Ordinal) => new(ToolKind.Mcp, tool, input),
            "skill" => new(ToolKind.Skill, tool, FirstOf(args, "name", "skill")),
            "read" or "view" or "read_file" => new(ToolKind.Read, tool, path),
            "write" or "edit" or "patch" or "multiedit" or "create" or "str_replace" or "apply_patch" or "edit_file" or "create_file" => new(ToolKind.Write, tool, path),
            "bash" or "shell" or "powershell" or "run_shell_command" => new(ToolKind.Shell, tool, FirstOf(args, "command", "cmd")),
            "glob" or "grep" or "list" or "ls" => new(ToolKind.Search, tool, FirstOf(args, "pattern", "path")),
            _ => new(ToolKind.Other, tool, Trunc(input, 400)),
        };
    }

    private static string FirstOf(JsonElement element, params string[] properties) =>
        properties.Select(p => Str(element, p)).FirstOrDefault(v => v.Length > 0) ?? "";


    private static IEnumerable<JsonElement> JsonLines(string stdout)
    {
        foreach (var line in stdout.Split('\n'))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith('{'))
            {
                continue;
            }

            JsonElement root;
            try
            {
                root = JsonDocument.Parse(trimmed).RootElement.Clone();
            }
            catch (JsonException)
            {
                continue;
            }

            yield return root;
        }
    }

    private static IEnumerable<JsonElement> Items(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Array ? value.EnumerateArray() : [];

    private static string ContentText(JsonElement part)
    {
        if (!part.TryGetProperty("content", out var content))
        {
            return "";
        }

        return content.ValueKind == JsonValueKind.String
            ? content.GetString() ?? ""
            : string.Join(" ", Items(part, "content").Select(c => Str(c, "text")));
    }

    private static string Str(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";

    private static string Raw(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) ? value.GetRawText() : "";

    public static string Trunc(string value, int max) =>
        value.Length <= max ? value.ReplaceLineEndings(" ") : value[..max].ReplaceLineEndings(" ") + "…";
}
