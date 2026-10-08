using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AgentEvals;

/// <summary>
/// Layer 1: static checks, no AI. For each harness folder the modules generated, is every hook,
/// instruction and skill somewhere that harness actually loads it? Load locations are taken from each
/// vendor's documentation (fetched 2026-10-07; see README.md). A harness often reads another harness's
/// folder - Copilot reads .claude/settings.json hooks, OpenCode reads .claude/skills - so locations
/// are checked across the whole generated output, not only the harness's own folder.
/// </summary>
public static class Wiring
{
    /// <summary>One hook the harness would run: its event, matcher and command text.</summary>
    private sealed record Registration(string Event, string Matcher, string Command);

    private sealed record Spec(
        string Id,
        string DisplayName,
        string Folder,
        Func<string, List<Registration>?> Hooks,
        string[] PreToolEvents,
        string[] StopEvents,
        Func<string, IEnumerable<string>> InstructionFiles,
        string[] SkillFolders,
        bool SkillNameMustMatchFolder,
        bool AlsoReadsClaudeHooks);

    private const string WorkflowHeading = "# Module Building Workflow";

    private static readonly string[] WorkflowSkills =
        ["module-context-capture", "module-version-increment", "module-docs-chore", "module-dependency-audit"];

    private static readonly Spec[] Specs =
    [
        new("claude", "Claude Code", ".claude",
            root => GroupedJson(root, ".claude/settings.json"),
            ["PreToolUse"], ["Stop"],
            root => Files(root, "CLAUDE.md", ".claude/CLAUDE.md").Concat(Glob(root, ".claude/rules", "*.md").Where(AlwaysApplies)),
            [".claude/skills"], SkillNameMustMatchFolder: false, AlsoReadsClaudeHooks: false),
        new("codex", "Codex", ".codex",
            root => GroupedJson(root, ".codex/hooks.json"),
            ["PreToolUse"], ["Stop"],
            // Codex has no instructions folder: besides AGENTS.md, only config.toml's developer_instructions.
            root => Files(root, "AGENTS.override.md", "AGENTS.md")
                .Concat(Files(root, ".codex/config.toml").Where(f => File.ReadAllText(f).Contains("developer_instructions", StringComparison.Ordinal))),
            [".agents/skills"], SkillNameMustMatchFolder: false, AlsoReadsClaudeHooks: false),
        new("copilot", "Copilot CLI", ".github",
            root => FlatJson(root, ".github/hooks", requiredVersion: "1"),
            ["preToolUse", "PreToolUse"], ["agentStop", "sessionEnd", "Stop"],
            // Copilot CLI also loads .claude/rules (observed with CLI 1.0.93; undocumented).
            root => Files(root, ".github/copilot-instructions.md", "AGENTS.md", "CLAUDE.md", ".claude/CLAUDE.md")
                .Concat(Glob(root, ".github/instructions", "*.instructions.md"))
                .Concat(Glob(root, ".claude/rules", "*.md").Where(AlwaysApplies)),
            [".github/skills", ".claude/skills", ".agents/skills"], SkillNameMustMatchFolder: false, AlsoReadsClaudeHooks: true),
        new("cursor", "Cursor", ".cursor",
            root => CursorJson(root),
            ["preToolUse", "beforeMCPExecution"], ["stop"],
            root => Files(root, "AGENTS.md", "CLAUDE.md").Concat(Glob(root, ".cursor/rules", "*.mdc")),
            [".agents/skills", ".cursor/skills", ".claude/skills", ".codex/skills"], SkillNameMustMatchFolder: true, AlsoReadsClaudeHooks: true),
        new("kiro", "Kiro", ".kiro",
            root => KiroJson(root),
            ["PreToolUse"], ["Stop"],
            root => Files(root, "AGENTS.md").Concat(Glob(root, ".kiro/steering", "*.md")),
            [".kiro/skills"], SkillNameMustMatchFolder: true, AlsoReadsClaudeHooks: false),
        new("opencode", "OpenCode", ".opencode",
            root => OpenCodePlugin(root),
            ["tool.execute.before"], ["session.idle", "session.stop"],
            root => Files(root, "AGENTS.md").Concat(OpenCodeConfiguredInstructions(root)),
            [".opencode/skills", ".opencode/skill", ".claude/skills", ".agents/skills"], SkillNameMustMatchFolder: true, AlsoReadsClaudeHooks: false),
    ];

    public static IReadOnlyList<string> Ids => Specs.Select(s => s.Id).ToList();

    public static async Task<IReadOnlyList<RunResult>> CheckAsync(EvalPaths paths, IReadOnlySet<string>? harnessFilter, string runDirectory)
    {
        var root = paths.GeneratedOutput;
        var gateBuilds = new Dictionary<string, (bool Ok, string Detail)>();
        var results = new List<RunResult>();

        foreach (var spec in Specs.Where(s => harnessFilter is null || harnessFilter.Contains(s.Id)))
        {
            var started = DateTime.UtcNow;
            if (!Directory.Exists(Path.Combine(root, spec.Folder)))
            {
                results.Add(new RunResult(spec.Id, "static", "W", "Wiring", [new Check("Generated", false, $"no {spec.Folder} folder in the generated output")], [], 0, 0, 0, null));
                continue;
            }

            var checks = new List<Check>();
            var registrations = spec.Hooks(root);
            checks.Add(registrations is null
                ? new Check("Hook config", false, "no hook configuration found, or it does not parse")
                : new Check("Hook config", true, $"{registrations.Count} hook(s) registered"));
            registrations ??= [];

            checks.Add(Wired(registrations, spec.PreToolEvents, "guard-write", null, "Write guard"));
            checks.Add(Wired(registrations, spec.PreToolEvents, "guard-version", "run_designer_script", "Version guard"));
            checks.Add(Wired(registrations, spec.StopEvents, "close-out", null, "Close-out"));
            checks.Add(await GateBuildsAsync(root, spec, registrations, gateBuilds, Path.Combine(runDirectory, "wiring", spec.Id)));

            if (spec.AlsoReadsClaudeHooks)
            {
                var claudeHooks = GroupedJson(root, ".claude/settings.json") ?? [];
                var gateHooks = claudeHooks.Count(r => r.Command.Contains("gate.cs", StringComparison.Ordinal));
                // Claude's copy steps aside when Copilot runs it: Copilot's Claude-format payload has no
                // transcript_path. Cursor's does, so under Cursor both copies still run (documented).
                var claudeGate = Path.Combine(root, ".claude", "hooks", "gate", "gate.cs");
                var stepsAside = spec.Id == "copilot" && File.Exists(claudeGate) && File.ReadAllText(claudeGate).Contains("StepsAsideFor", StringComparison.Ordinal);
                checks.Add(new Check("No duplicate hooks", gateHooks == 0 || stepsAside,
                    gateHooks == 0 ? "no other harness's gate hooks are loaded too"
                    : stepsAside ? $"{spec.DisplayName} also loads {gateHooks} gate hook(s) from .claude/settings.json; Claude's copy steps aside when {spec.DisplayName} runs it (verified by D1)"
                    : $"{spec.DisplayName} also loads .claude/settings.json hooks, so {gateHooks} gate hook(s) run a second time, as Claude Code - a documented limitation"));
            }

            var instructionFiles = spec.InstructionFiles(root).Distinct().ToList();
            var carrier = instructionFiles.FirstOrDefault(f => File.ReadAllText(f).Contains(WorkflowHeading, StringComparison.Ordinal));
            checks.Add(new Check("Instructions reach", carrier is not null,
                carrier is not null ? $"workflow instructions load from {Relative(root, carrier)}"
                : instructionFiles.Count == 0 ? "nothing in any location this harness auto-loads instructions from"
                : $"auto-loaded files ({string.Join(", ", instructionFiles.Select(f => Relative(root, f)))}) do not carry the workflow instructions"));

            var missing = new List<string>();
            foreach (var skill in WorkflowSkills)
            {
                var found = spec.SkillFolders.Select(folder => Path.Combine(root, folder, skill, "SKILL.md")).FirstOrDefault(File.Exists);
                if (found is null || !ValidSkill(found, skill, spec.SkillNameMustMatchFolder))
                {
                    missing.Add(skill);
                }
            }

            checks.Add(new Check("Skills reachable", missing.Count == 0,
                missing.Count == 0 ? $"all {WorkflowSkills.Length} workflow skills load from {string.Join(" or ", spec.SkillFolders)}"
                : $"not loadable: {string.Join(", ", missing)} (looked in {string.Join(", ", spec.SkillFolders)})"));

            results.Add(new RunResult(spec.Id, "static", "W", "Wiring", checks, [], 0, 0, (DateTime.UtcNow - started).TotalSeconds, null));
        }

        return results;
    }

    private static Check Wired(List<Registration> registrations, string[] events, string command, string? matcherMustCover, string dimension)
    {
        var hit = registrations.FirstOrDefault(r =>
            events.Contains(r.Event, StringComparer.Ordinal)
            // "guard-tool" is one hook that dispatches to both guards by tool name (Copilot).
            && (r.Command.Contains(command, StringComparison.Ordinal) || r.Command.Contains("guard-tool", StringComparison.Ordinal))
            && (matcherMustCover is null || r.Matcher.Length == 0 || Covers(r.Matcher, matcherMustCover)));
        return hit is not null
            ? new Check(dimension, true, $"{command} on {hit.Event}{(hit.Matcher.Length > 0 ? $" ({hit.Matcher})" : "")}")
            : new Check(dimension, false, $"no {command} hook on {string.Join("/", events)}{(matcherMustCover is null ? "" : $" for {matcherMustCover}")}");
    }

    private static bool Covers(string matcher, string toolName)
    {
        try
        {
            return Regex.IsMatch(toolName, matcher) || Regex.IsMatch($"mcp__intent-architect__{toolName}", matcher);
        }
        catch (ArgumentException)
        {
            return matcher.Contains(toolName, StringComparison.Ordinal);
        }
    }

    private static async Task<Check> GateBuildsAsync(string root, Spec spec, List<Registration> registrations, Dictionary<string, (bool Ok, string Detail)> cache, string scratch)
    {
        var gatePath = registrations.Select(r => Regex.Match(r.Command, @"([\w./-]+/gate\.cs)").Groups[1].Value).FirstOrDefault(p => p.Length > 0);
        if (gatePath is null)
        {
            return new Check("Gate builds", false, "no hook names a gate.cs");
        }

        // OpenCode's plugin builds the path from process.cwd(), so what is left starts with "/".
        gatePath = gatePath.TrimStart('/');
        var gateFile = Path.Combine(root, gatePath);
        if (!File.Exists(gateFile))
        {
            return new Check("Gate builds", false, $"{gatePath} does not exist");
        }

        // Every copy is the same file today; build each distinct one once. The build runs on a copy,
        // so the generated output is only ever read.
        var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(gateFile)))[..12];
        if (!cache.TryGetValue(hash, out var outcome))
        {
            var copy = Path.Combine(scratch, "gate");
            Workspace.CopyDirectory(Path.GetDirectoryName(gateFile)!, copy);
            var warm = await Processes.RunAsync(Processes.Command("dotnet", copy, "run", "gate.cs", "--", "warm"), null, TimeSpan.FromMinutes(3));
            outcome = warm.ExitCode == 0
                ? (true, "builds and answers warm")
                : (false, $"warm failed: {TranscriptParsers.Trunc(warm.Stderr + warm.Stdout, 300)}");
            cache[hash] = outcome;
        }

        return new Check("Gate builds", outcome.Ok, $"{gatePath}: {outcome.Detail}");
    }

    // --- hook config readers: each returns null when the config is missing or does not parse ---

    /// <summary>Claude Code and Codex: { hooks: { Event: [ { matcher, hooks: [ { command } ] } ] } }.</summary>
    private static List<Registration>? GroupedJson(string root, string relative)
    {
        var json = Parse(Path.Combine(root, relative));
        if (json is not { } doc || !doc.TryGetProperty("hooks", out var hooks) || hooks.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var list = new List<Registration>();
        foreach (var ev in hooks.EnumerateObject())
        {
            foreach (var group in Array(ev.Value))
            {
                var matcher = Str(group, "matcher");
                foreach (var hook in Array(group, "hooks"))
                {
                    list.Add(new Registration(ev.Name, matcher, Str(hook, "command") + " " + Str(hook, "commandWindows")));
                }
            }
        }

        return list;
    }

    /// <summary>Copilot CLI: .github/hooks/*.json, { version: 1, hooks: { event: [ { bash, powershell } ] } }.</summary>
    private static List<Registration>? FlatJson(string root, string folder, string requiredVersion)
    {
        var files = Glob(root, folder, "*.json").ToList();
        if (files.Count == 0)
        {
            return null;
        }

        var list = new List<Registration>();
        foreach (var file in files)
        {
            if (Parse(file) is not { } doc || Raw(doc, "version") != requiredVersion || !doc.TryGetProperty("hooks", out var hooks))
            {
                return null;
            }

            foreach (var ev in hooks.EnumerateObject())
            {
                foreach (var hook in Array(ev.Value))
                {
                    list.Add(new Registration(ev.Name, Str(hook, "matcher"), Str(hook, "bash") + " " + Str(hook, "powershell") + " " + Str(hook, "command")));
                }
            }
        }

        return list;
    }

    private static List<Registration>? CursorJson(string root) => FlatJsonFile(root, ".cursor/hooks.json", "1");

    private static List<Registration>? FlatJsonFile(string root, string relative, string requiredVersion)
    {
        if (Parse(Path.Combine(root, relative)) is not { } doc || Raw(doc, "version") != requiredVersion || !doc.TryGetProperty("hooks", out var hooks))
        {
            return null;
        }

        return hooks.EnumerateObject()
            .SelectMany(ev => Array(ev.Value).Select(hook => new Registration(ev.Name, Str(hook, "matcher"), Str(hook, "command"))))
            .ToList();
    }

    /// <summary>Kiro: .kiro/hooks/*.json, { version: "v1", hooks: [ { trigger, matcher, action: { command } } ] }.</summary>
    private static List<Registration>? KiroJson(string root)
    {
        var files = Glob(root, ".kiro/hooks", "*.json").ToList();
        if (files.Count == 0)
        {
            return null;
        }

        var list = new List<Registration>();
        foreach (var file in files)
        {
            if (Parse(file) is not { } doc || Str(doc, "version") != "v1")
            {
                return null;
            }

            foreach (var hook in Array(doc, "hooks"))
            {
                var command = hook.TryGetProperty("action", out var action) ? Str(action, "command") : "";
                list.Add(new Registration(Str(hook, "trigger"), Str(hook, "matcher"), command));
            }
        }

        return list;
    }

    /// <summary>OpenCode has no hook file: the plugin's code is read for the gate commands it runs, and on which event.</summary>
    private static List<Registration>? OpenCodePlugin(string root)
    {
        var files = Glob(root, ".opencode/plugins", "*.ts").Concat(Glob(root, ".opencode/plugins", "*.js")).ToList();
        if (files.Count == 0)
        {
            return null;
        }

        var list = new List<Registration>();
        foreach (var text in files.Select(File.ReadAllText))
        {
            var gate = Regex.Match(text, @"[\w${}()./-]*/gate\.cs").Value;
            if (text.Contains("\"tool.execute.before\"", StringComparison.Ordinal))
            {
                foreach (Match call in Regex.Matches(text, "runGuard\\(\"([a-z-]+)\""))
                {
                    var matcher = call.Groups[1].Value == "guard-version" ? "run_designer_script" : "write|edit";
                    list.Add(new Registration("tool.execute.before", matcher, $"{gate} {call.Groups[1].Value}"));
                }
            }

            // Close-out runs from the generic "event" hook when the session goes idle.
            if (Regex.IsMatch(text, "\"session\\.(idle|status)\"") && text.Contains("runCloseOut(", StringComparison.Ordinal))
            {
                list.Add(new Registration("session.idle", "", $"{gate} close-out"));
            }
        }

        return list;
    }

    private static IEnumerable<string> OpenCodeConfiguredInstructions(string root)
    {
        foreach (var name in new[] { "opencode.json", "opencode.jsonc", ".opencode/opencode.json" })
        {
            if (Parse(Path.Combine(root, name)) is { } doc)
            {
                foreach (var entry in Array(doc, "instructions"))
                {
                    var pattern = entry.GetString() ?? "";
                    var folder = Path.GetDirectoryName(pattern) ?? "";
                    foreach (var file in Glob(root, folder, Path.GetFileName(pattern)))
                    {
                        yield return file;
                    }
                }
            }
        }
    }

    // --- helpers ---

    private static bool AlwaysApplies(string file) => !Frontmatter(File.ReadAllText(file)).ContainsKey("paths");

    private static bool ValidSkill(string skillFile, string folderName, bool nameMustMatch)
    {
        var frontmatter = Frontmatter(File.ReadAllText(skillFile));
        return frontmatter.TryGetValue("name", out var name) && name.Length > 0
            && frontmatter.TryGetValue("description", out var description) && description.Length > 0
            && (!nameMustMatch || name == folderName);
    }

    private static Dictionary<string, string> Frontmatter(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var match = Regex.Match(text, @"\A---\r?\n(.*?)\r?\n---", RegexOptions.Singleline);
        if (match.Success)
        {
            foreach (Match line in Regex.Matches(match.Groups[1].Value, @"^([\w-]+):\s*(.*)$", RegexOptions.Multiline))
            {
                values[line.Groups[1].Value] = line.Groups[2].Value.Trim().Trim('"');
            }
        }

        return values;
    }

    private static IEnumerable<string> Files(string root, params string[] relative) =>
        relative.Select(r => Path.Combine(root, r)).Where(File.Exists);

    private static IEnumerable<string> Glob(string root, string folder, string pattern)
    {
        var path = Path.Combine(root, folder);
        return Directory.Exists(path) ? Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories) : [];
    }

    private static JsonElement? Parse(string path)
    {
        try
        {
            return File.Exists(path) ? JsonDocument.Parse(File.ReadAllText(path)).RootElement.Clone() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static IEnumerable<JsonElement> Array(JsonElement element, string? property = null)
    {
        var value = element;
        if (property is not null && (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(property, out value)))
        {
            return [];
        }

        return value.ValueKind == JsonValueKind.Array ? value.EnumerateArray() : [];
    }

    private static string Str(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : "";

    private static string Raw(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) ? value.ToString() : "";

    private static string Relative(string root, string file) => Path.GetRelativePath(root, file).Replace('\\', '/');
}
