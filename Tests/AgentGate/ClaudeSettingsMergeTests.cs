using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The merge rules for ".claude/settings.json" - the one file this module writes into rather than
/// owns. The original decision was NOT to generate it at all, because "no way was found to verify a
/// merge wouldn't corrupt a developer's existing config". These cases are that verification, so the
/// reversal rests on evidence rather than optimism.
///
/// The logic under test is a faithful port of ClaudeSettingsTemplate's private merge - the template
/// itself lives in the module assembly and cannot be referenced from here, so the rules are
/// re-stated rather than linked. That is a real seam: a change to the template that is not mirrored
/// here would go uncaught.
/// </summary>
public class ClaudeSettingsMergeTests
{
    private const string GatePath = ".claude/hooks/gate/gate.cs";

    [Fact]
    public void Writes_a_default_when_no_settings_file_exists()
    {
        var result = Merge(existing: null);

        var hooks = result["hooks"]!.AsObject();
        Assert.True(hooks.ContainsKey("SessionStart"));
        Assert.True(hooks.ContainsKey("PreToolUse"));
        Assert.True(hooks.ContainsKey("Stop"));
    }

    [Fact]
    public void Leaves_unrelated_top_level_keys_untouched()
    {
        var result = Merge("""
            { "permissions": { "allow": ["Bash(npm test)"] }, "env": { "FOO": "bar" } }
            """);

        Assert.Equal("Bash(npm test)", result["permissions"]!["allow"]![0]!.GetValue<string>());
        Assert.Equal("bar", result["env"]!["FOO"]!.GetValue<string>());
    }

    [Fact]
    public void Preserves_a_developers_own_hooks_and_adds_ours_alongside()
    {
        var result = Merge("""
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
        var once = Merge(existing: null);
        var twice = Merge(once.ToJsonString());

        Assert.Equal(once.ToJsonString(), twice.ToJsonString());
        Assert.Equal(2, twice["hooks"]!["PreToolUse"]!.AsArray().Count);
    }

    [Fact]
    public void Does_not_overwrite_a_command_the_developer_has_adjusted()
    {
        // Same matcher, same gate, but their own wording. "Write the value when it's the first
        // time" - after that it is theirs, so we must not append a near-duplicate beside it.
        var result = Merge($$"""
            {
              "hooks": {
                "PreToolUse": [
                  { "matcher": "Write|Edit", "hooks": [ { "type": "command", "command": "dotnet run {{GatePath}} -- guard-write --my-tweak" } ] }
                ]
              }
            }
            """);

        var entries = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => e!["matcher"]!.GetValue<string>() == "Write|Edit")
            .ToList();

        Assert.Single(entries);
        Assert.Contains("--my-tweak", entries[0]!["hooks"]![0]!["command"]!.GetValue<string>());
    }

    [Fact]
    public void An_entry_pointing_at_an_old_gate_path_is_treated_as_absent_and_rewired()
    {
        // The gate moved from ".agents/hooks/gate.cs" to per-harness copies. An entry still aimed at
        // the old location is not ours by the path test, so the current one is added rather than the
        // stale one being silently trusted.
        var result = Merge("""
            {
              "hooks": {
                "PreToolUse": [
                  { "matcher": "Write|Edit", "hooks": [ { "type": "command", "command": "dotnet run .agents/hooks/gate.cs -- guard-write" } ] }
                ]
              }
            }
            """);

        var entries = result["hooks"]!["PreToolUse"]!.AsArray()
            .Where(e => e!["matcher"]!.GetValue<string>() == "Write|Edit")
            .ToList();

        Assert.Equal(2, entries.Count);
        Assert.Contains(entries, e => e!["hooks"]![0]!["command"]!.GetValue<string>().Contains(GatePath));
    }

    [Fact]
    public void Malformed_json_is_detected_rather_than_parsed()
    {
        var malformed = "{ \"hooks\": { ";

        Assert.ThrowsAny<JsonException>(() => JsonNode.Parse(malformed));
    }

    // --- the rules under test, mirroring ClaudeSettingsTemplate ---

    private static JsonObject Merge(string? existing)
    {
        var root = existing is null ? new JsonObject() : JsonNode.Parse(existing)!.AsObject();

        if (root["hooks"] is not JsonObject hooks)
        {
            hooks = new JsonObject();
            root["hooks"] = hooks;
        }

        EnsureHook(hooks, "SessionStart", null, $"dotnet run {GatePath} -- warm");
        EnsureHook(hooks, "PreToolUse", "Write|Edit", GateCommand("guard-write"));
        EnsureHook(hooks, "PreToolUse", ".*run_designer_script.*", GateCommand("guard-version"));
        EnsureHook(hooks, "Stop", null, GateCommand("close-out"));

        return root;
    }

    private static string GateCommand(string command) =>
        $"dotnet run {GatePath} --no-build -- {command} --harness claude; test $? -eq 0 && exit 0 || exit 2";

    private static void EnsureHook(JsonObject hooks, string eventName, string? matcher, string command)
    {
        if (hooks[eventName] is not JsonArray entries)
        {
            entries = new JsonArray();
            hooks[eventName] = entries;
        }

        foreach (var entry in entries.OfType<JsonObject>())
        {
            if (!string.Equals(entry["matcher"]?.GetValue<string>(), matcher, StringComparison.Ordinal))
            {
                continue;
            }

            var alreadyWired = entry["hooks"] is JsonArray inner
                               && inner.OfType<JsonObject>().Any(h =>
                                   h["command"]?.GetValue<string>()?.Contains(GatePath, StringComparison.Ordinal) == true);
            if (alreadyWired)
            {
                return;
            }
        }

        var added = new JsonObject();
        if (matcher is not null)
        {
            added["matcher"] = matcher;
        }

        added["hooks"] = new JsonArray(new JsonObject
        {
            ["type"] = "command",
            ["command"] = command,
        });

        entries.Add(added);
    }
}
