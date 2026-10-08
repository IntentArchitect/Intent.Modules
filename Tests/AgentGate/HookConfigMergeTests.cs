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
        Assert.Contains(preToolUse, e => e!["matcher"]!.GetValue<string>() == "Write|Edit");
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
            .Where(e => e!["matcher"]!.GetValue<string>() == "Write|Edit")
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
            .Where(e => e!["matcher"]!.GetValue<string>() == "Write|Edit")
            .ToList();

        Assert.Single(entries);
        Assert.Equal(GateCommands.GuardPosix(".claude", "guard-write"), CommandOf(entries[0]!));
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
        Assert.Equal("apply_patch|Edit|Write", writeGuards[0]!["matcher"]!.GetValue<string>());
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
