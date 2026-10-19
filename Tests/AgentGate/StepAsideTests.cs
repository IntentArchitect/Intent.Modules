using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Copilot CLI (and Cursor) also run the hooks in .claude/settings.json, so Claude Code's copy of the gate
/// steps aside when another harness that has its own hook ran it. It must never step aside for Claude
/// Code itself: a copy wrongly stepping aside is a guard switched off.
/// </summary>
/// <remarks>
/// Only the "claude" harness id is ever subject to this, and no other test uses it, so overriding the
/// process-tree probe here cannot leak into tests running alongside.
/// </remarks>
public class StepAsideTests
{
    [Fact]
    public void Steps_aside_when_copilot_runs_claudes_hook_with_its_own_payload()
    {
        var result = Run(runner: "copilot", withTranscriptPath: false);

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
        Assert.Contains("stepping aside", result.Stderr);
    }

    [Fact]
    public void Decides_when_claude_code_runs_its_own_hook()
    {
        var result = Run(runner: "claude", withTranscriptPath: true);

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Decides_when_claude_code_was_started_from_a_copilot_session()
    {
        // The process tree alone points at Copilot here; Claude Code's own payload says otherwise.
        var result = Run(runner: "copilot", withTranscriptPath: true);

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Decides_when_the_harness_cannot_be_recognised()
    {
        var result = Run(runner: null, withTranscriptPath: false);

        Assert.Equal(2, result.ExitCode);
    }

    private static GateResult Run(string? runner, bool withTranscriptPath)
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var config = repo.CreateFile("App/App.application.config", "<application />");

        var payload = new Dictionary<string, object?>
        {
            ["hook_event_name"] = "PreToolUse",
            ["session_id"] = "s",
            ["tool_name"] = "Edit",
            ["tool_input"] = new { file_path = config, new_string = "<application />" },
        };
        if (withTranscriptPath)
        {
            payload["transcript_path"] = "C:/transcript.jsonl";
        }

        var original = HostEnvironment.NearestHarness;
        HostEnvironment.NearestHarness = () => runner;
        try
        {
            return GateTestHarness.Run(repo.Path, JsonSerializer.Serialize(payload), null, "guard-write", "--harness", "claude");
        }
        finally
        {
            HostEnvironment.NearestHarness = original;
        }
    }
}
