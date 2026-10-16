using System.Text.Json.Nodes;
using AgentGate.Tests.TestSupport;
using Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The merge rules for the hook files this module writes INTO rather than owns -
/// ".claude/settings.json", ".codex/hooks.json" and ".cursor/hooks.json" - run against the module's
/// own HookConfigMerge and HarnessHookSets sources, compiled into this project as they are.
/// </summary>
/// <remarks>
/// The original decision was not to generate ".claude/settings.json" at all, because no way was
/// found to verify a merge wouldn't corrupt a developer's existing config. These cases are that
/// verification. Codex and Cursor joined when it turned out owning their files outright deleted
/// every other tool's hooks on each Software Factory run.
/// </remarks>
public class HookConfigMergeTests
{
    private const string ClaudeGate = ".claude/hooks/gate/gate.cs";

    /// <summary>guard-write covers Intent's MCP file tools as well as Claude Code's own edit tools.</summary>
    private const string ClaudeWriteMatcher = HarnessHookSets.ClaudeWriteMatcher;

    // --- .claude/settings.json ---

    [Fact]
    public void Writes_a_default_when_no_settings_file_exists()
    {
        var result = MergeClaude(existing: null);

        var hooks = result["hooks"]!.AsObject();
        Assert.True(hooks.ContainsKey("SessionStart"));
        Assert.True(hooks.ContainsKey("PreToolUse"));
        Assert.True(hooks.ContainsKey("Stop"));
    }

    [Fact]
    public void Leaves_unrelated_top_level_keys_untouched()
    {
        var result = MergeClaude("""
            { "permissions": { "allow": ["Bash(npm test)"] }, "env": { "FOO": "bar" } }
            """);

        Assert.Equal("Bash(npm test)", result["permissions"]!["allow"]![0]!.GetValue<string>());
        Assert.Equal("bar", result["env"]!["FOO"]!.GetValue<string>());
    }

    [Fact]
    public void Preserves_a_developers_own_hooks_and_adds_ours_alongside()
    {
        var result = MergeClaude("""
            {
              "hooks": {
                "PreToolUse": [
                  { "matcher": "Bash", "hooks": [ { "type": "command", "command": "echo mine" } ] }
                ]
              }
            }
            """);

        var preToolUse = result["hooks"]!["PreToolUse"]!.AsArray();
        Assert.Contains(preToolUse, e => e!["matcher"]!.GetValue<string>() == "Bash");
        Assert.Contains(preToolUse, e => e!["matcher"]!.GetValue<string>() == ClaudeWriteMatcher);
    }

    [Fact]
    public void Is_idempotent_a_second_run_adds_nothing()
    {
        var once = MergeClaude(existing: null);
        var twice = MergeClaude(once.ToJsonString());

        Assert.Equal(once.ToJsonString(), twice.ToJsonString());
        Assert.Equal(2, twice["hooks"]!["PreToolUse"]!.AsArray().Count);
    }

    [Fact]
    public void Does_not_overwrite_a_command_the_developer_has_adjusted()
    {
        // Same gate, same subcommand, but their own wording. After the first write it is theirs, so
        // we must neither rewrite it nor append a near-duplicate beside it.
        var theirs = $"dotnet run {ClaudeGate} -- guard-write --my-tweak";
        var result = MergeClaude(Grouped("PreToolUse", "Write|Edit", theirs));

        var entries = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => e!["matcher"]!.GetValue<string>() == "Write|Edit")
            .ToList();

        Assert.Single(entries);
        Assert.Equal(theirs, CommandOf(entries[0]!));
    }

    [Fact]
    public void An_entry_pointing_at_an_old_gate_path_is_treated_as_absent_and_rewired()
    {
        // The gate moved from ".agents/hooks/gate.cs" to per-harness copies. An entry still aimed at
        // the old location is not ours by the path test, so the current one is added rather than the
        // stale one being silently trusted.
        var result = MergeClaude(Grouped("PreToolUse", "Write|Edit", "dotnet run .agents/hooks/gate.cs -- guard-write"));

        var entries = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => CommandOf(e!).Contains("guard-write"))
            .ToList();

        Assert.Equal(2, entries.Count);
        Assert.Contains(entries, e => CommandOf(e!).Contains(ClaudeGate));
    }

    [Theory]
    [InlineData(0)] // the shared "; test $? ..." wrapper every harness used up to 1.0.0-pre.8
    [InlineData(1)] // the brief "${CLAUDE_PROJECT_DIR}" anchoring
    public void Upgrades_a_guard_this_module_generated_in_an_earlier_version(int supersededIndex)
    {
        // The merge can only ADD absent entries, so without this a change to the command itself would
        // reach new installs only - every existing settings.json would keep the old form for good.
        var old = GateCommands.Superseded(".claude", "guard-write")[supersededIndex];
        var result = MergeClaude(Grouped("PreToolUse", "Write|Edit", old));

        var entries = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => CommandOf(e!).Contains("guard-write"))
            .ToList();

        Assert.Single(entries);
        Assert.Equal(ClaudeWriteMatcher, entries[0]!["matcher"]!.GetValue<string>());
        Assert.Equal(GateCommands.GuardPosix(".claude", "guard-write"), CommandOf(entries[0]!));
    }

    [Fact]
    public void Widens_an_existing_write_guard_to_cover_intent_file_tools_without_duplicating_it()
    {
        // Every install up to 1.0.0-pre.10 has the CURRENT command under "Write|Edit". Found by its
        // command, the entry takes the wider matcher in place - no second guard-write beside it.
        var result = MergeClaude(Grouped("PreToolUse", "Write|Edit", GateCommands.GuardPosix(".claude", "guard-write")));

        var entries = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => CommandOf(e!).Contains("guard-write"))
            .ToList();

        Assert.Single(entries);
        Assert.Equal(ClaudeWriteMatcher, entries[0]!["matcher"]!.GetValue<string>());
    }

    [Theory]
    [InlineData("mcp__intent-architect__write_file", true)]
    [InlineData("mcp__ia__patch_file", true)]
    [InlineData("mcp__intent-architect__delete_code_file", true)]
    [InlineData("Write", true)]
    [InlineData("Edit", true)]
    [InlineData("mcp__intent-architect__read_file", false)]
    [InlineData("mcp__intent-architect__run_designer_script", false)]
    [InlineData("TodoWrite", false)]
    [InlineData("NotebookEdit", false)]
    [InlineData("MultiEdit", false)]
    public void The_write_matcher_selects_intent_file_tools_whatever_the_server_is_called(string toolName, bool matched)
    {
        // Claude Code tests a matcher that holds any regex character as an UNANCHORED JavaScript regex
        // (RegExp.prototype.test), so this must hold for a search, not a whole-string match.
        Assert.Equal(matched, System.Text.RegularExpressions.Regex.IsMatch(toolName, ClaudeWriteMatcher));
    }

    [Fact]
    public void Moves_our_hook_out_of_a_group_it_shares_with_the_developers_own_hook()
    {
        // Rewriting the shared group's matcher would change when the developer's hook runs.
        var existing = new JsonObject
        {
            ["hooks"] = new JsonObject
            {
                ["PreToolUse"] = new JsonArray(new JsonObject
                {
                    ["matcher"] = "Write|Edit",
                    ["hooks"] = new JsonArray(
                        new JsonObject { ["type"] = "command", ["command"] = GateCommands.GuardPosix(".claude", "guard-write") },
                        new JsonObject { ["type"] = "command", ["command"] = "./scripts/check-header.sh" }),
                }),
            },
        }.ToJsonString();

        var once = MergeClaude(existing);
        var twice = MergeClaude(once.ToJsonString());
        var preToolUse = twice["hooks"]!["PreToolUse"]!.AsArray();

        var developers = Assert.Single(preToolUse, e => e!["hooks"]!.AsArray().Any(h => h!["command"]!.GetValue<string>() == "./scripts/check-header.sh"));
        Assert.Equal("Write|Edit", developers!["matcher"]!.GetValue<string>());
        Assert.Single(developers["hooks"]!.AsArray());
        var ours = Assert.Single(preToolUse, e => CommandOf(e!).Contains("guard-write"));
        Assert.Equal(ClaudeWriteMatcher, ours!["matcher"]!.GetValue<string>());
        Assert.Equal(once.ToJsonString(), twice.ToJsonString());
    }

    [Fact]
    public void Close_out_loses_its_fail_closed_wrapper_on_upgrade()
    {
        // A Stop hook exiting 2 makes Claude Code keep going, so the wrapped close-out turned a gate
        // that could not run into an endless loop of turns. Existing installs must be carried off it.
        var old = GateCommands.Superseded(".claude", "close-out")[0];
        var result = MergeClaude(Grouped("Stop", null, old));

        var stop = result["hooks"]!["Stop"]!.AsArray();
        Assert.Single(stop);
        Assert.Equal(GateCommands.CloseOut(".claude"), CommandOf(stop[0]!));
    }

    [Fact]
    public void Keeps_claude_code_in_the_project_directory_between_commands()
    {
        // Claude Code runs hooks in the shell's current directory; after the agent "cd"s into a module
        // folder the relative gate path no longer resolves and every guarded write is blocked.
        var result = MergeClaude(existing: null);

        Assert.Equal("1", result["env"]![HarnessHookSets.MaintainProjectWorkingDirectory]!.GetValue<string>());
    }

    [Fact]
    public void Leaves_the_developers_own_environment_settings_alone()
    {
        var result = MergeClaude($$"""
            { "env": { "FOO": "bar", "{{HarnessHookSets.MaintainProjectWorkingDirectory}}": "0" } }
            """);

        Assert.Equal("bar", result["env"]!["FOO"]!.GetValue<string>());
        Assert.Equal("0", result["env"]![HarnessHookSets.MaintainProjectWorkingDirectory]!.GetValue<string>());
    }

    [Fact]
    public void Only_claude_code_gets_the_working_directory_setting()
    {
        Assert.Null(MergeCodex(existing: null)["env"]);
        Assert.Null(MergeCursor(existing: null)["env"]);
    }

    // --- .codex/hooks.json ---

    [Fact]
    public void Codex_keeps_another_tools_hooks()
    {
        var result = MergeCodex(Grouped("SessionStart", null, "C:/tools/memory.exe hook-augment"));

        var sessionStart = result["hooks"]!["SessionStart"]!.AsArray();
        Assert.Contains(sessionStart, e => CommandOf(e!) == "C:/tools/memory.exe hook-augment");
        Assert.Contains(sessionStart, e => CommandOf(e!) == GateCommands.Warm(".codex"));
    }

    [Fact]
    public void Codex_rewrites_its_old_guard_and_matcher_in_place_rather_than_duplicating_it()
    {
        // The old matcher was "Write|Edit|ApplyPatch". Keying the upgrade on the matcher would have
        // left that stale entry - and its POSIX-only command - running beside the new one.
        var old = GateCommands.Superseded(".codex", "guard-write")[0];
        var result = MergeCodex(Grouped("PreToolUse", "Write|Edit|ApplyPatch", old));

        var writeGuards = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => CommandOf(e!).Contains("guard-write"))
            .ToList();

        Assert.Single(writeGuards);
        Assert.Equal($"apply_patch|Edit|Write|{GateCommands.IntentFileToolsMatcher}", writeGuards[0]!["matcher"]!.GetValue<string>());
        Assert.Equal(GateCommands.GuardPosix(".codex", "guard-write"), CommandOf(writeGuards[0]!));
    }

    [Fact]
    public void Codex_guards_carry_a_powershell_command_for_windows()
    {
        // Codex runs "commandWindows" in PowerShell on Windows, where the POSIX wrapper does not parse.
        var result = MergeCodex(existing: null);

        foreach (var entry in result["hooks"]!["PreToolUse"]!.AsArray())
        {
            var hook = entry!["hooks"]![0]!;
            Assert.Contains("$LASTEXITCODE", hook["commandWindows"]!.GetValue<string>());
        }
    }

    // --- .cursor/hooks.json ---

    [Fact]
    public void Cursor_keeps_another_tools_hooks_and_adds_a_version()
    {
        var result = MergeCursor("""
            { "hooks": { "preToolUse": [ { "type": "command", "command": "./audit.sh", "matcher": "Shell" } ] } }
            """);

        Assert.Equal(1, result["version"]!.GetValue<int>());
        var preToolUse = result["hooks"]!["preToolUse"]!.AsArray();
        Assert.Contains(preToolUse, h => h!["command"]!.GetValue<string>() == "./audit.sh");
        Assert.Contains(preToolUse, h => h!["command"]!.GetValue<string>() == GateCommands.GuardPortable(".cursor", "guard-write"));
    }

    [Fact]
    public void Cursor_upgrades_its_old_guard_and_keeps_it_failing_closed()
    {
        var old = GateCommands.Superseded(".cursor", "guard-write")[0];
        var result = MergeCursor($$"""
            { "version": 1, "hooks": { "preToolUse": [ { "type": "command", "command": {{JsonValue.Create(old).ToJsonString()}}, "matcher": "Write|Delete", "failClosed": true } ] } }
            """);

        var guard = Assert.Single(result["hooks"]!["preToolUse"]!.AsArray());
        Assert.Equal(GateCommands.GuardPortable(".cursor", "guard-write"), guard!["command"]!.GetValue<string>());
        Assert.True(guard["failClosed"]!.GetValue<bool>());
    }

    [Fact]
    public void Cursor_guards_intent_file_tools_before_mcp_execution_failing_closed()
    {
        // Cursor routes MCP calls through beforeMCPExecution, not preToolUse's Write category.
        var mcp = MergeCursor(null)["hooks"]!["beforeMCPExecution"]!.AsArray();

        var guard = Assert.Single(mcp, h => h!["command"]!.GetValue<string>().Contains("guard-write"));
        Assert.Equal(GateCommands.GuardPortable(".cursor", "guard-write"), guard!["command"]!.GetValue<string>());
        Assert.Equal(GateCommands.IntentFileToolsMatcher, guard["matcher"]!.GetValue<string>());
        Assert.True(guard["failClosed"]!.GetValue<bool>());
        Assert.Single(mcp, h => h!["command"]!.GetValue<string>().Contains("guard-version"));
    }

    // --- every harness ---

    [Fact]
    public void No_close_out_command_is_ever_wrapped_to_fail_closed()
    {
        Assert.Equal(GateCommands.CloseOut(".claude"), CommandOf(MergeClaude(null)["hooks"]!["Stop"]![0]!));
        Assert.Equal(GateCommands.CloseOut(".codex"), CommandOf(MergeCodex(null)["hooks"]!["Stop"]![0]!));
        Assert.Equal(GateCommands.CloseOut(".cursor"), MergeCursor(null)["hooks"]!["stop"]![0]!["command"]!.GetValue<string>());
        Assert.DoesNotContain("exit", GateCommands.CloseOut(".claude"));
    }

    [Fact]
    public void A_file_that_is_not_a_json_object_is_reported_unusable_and_not_rewritten()
    {
        using var directory = new TempDirectory();
        var path = directory.CreateFile("hooks.json", "{ \"hooks\": { ");

        var outcome = HookConfigMerge.Load(path, out _, out var existing, out var problem);

        Assert.Equal(HookConfigMerge.LoadOutcome.Unusable, outcome);
        Assert.Equal("{ \"hooks\": { ", existing);
        Assert.Contains("not valid JSON", problem);
    }

    private static JsonObject MergeClaude(string? existing) => Merge(existing, HarnessHookSets.MergeClaudeSettings);

    private static JsonObject MergeCodex(string? existing) => Merge(existing, HarnessHookSets.MergeCodexHooks);

    private static JsonObject MergeCursor(string? existing) => Merge(existing, HarnessHookSets.MergeCursorHooks);

    private static JsonObject Merge(string? existing, Action<JsonObject> merge)
    {
        var root = existing is null ? new JsonObject() : JsonNode.Parse(existing)!.AsObject();
        merge(root);
        // Round-trip through the real renderer, so what is asserted is what would be written.
        return JsonNode.Parse(HookConfigMerge.Render(root))!.AsObject();
    }

    /// <summary>A grouped (Claude Code / Codex shaped) file with a single hook.</summary>
    private static string Grouped(string eventName, string? matcher, string command)
    {
        var entry = new JsonObject();
        if (matcher is not null)
        {
            entry["matcher"] = matcher;
        }

        entry["hooks"] = new JsonArray(new JsonObject { ["type"] = "command", ["command"] = command });
        return new JsonObject { ["hooks"] = new JsonObject { [eventName] = new JsonArray(entry) } }.ToJsonString();
    }

    private static string CommandOf(JsonNode groupedEntry) => groupedEntry["hooks"]![0]!["command"]!.GetValue<string>();
}
