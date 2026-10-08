using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// INTENT_GATE_LOG: the opt-in log the agent-gate playpen reads to see every hook run the same way on
/// every harness. It must record faithfully and must never change a verdict.
/// </summary>
public class GateTelemetryTests
{
    [Fact]
    public void Records_each_run_without_changing_its_verdict_or_output()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var log = Path.Combine(repo.Path, "gate-telemetry.jsonl");
        var stdin = "{\"tool_input\":{\"file_path\":\"Intent.Metadata/element.xml\"}}";

        GateResult result;
        Environment.SetEnvironmentVariable(GateTelemetry.EnvironmentVariable, log);
        try
        {
            result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");
        }
        finally
        {
            Environment.SetEnvironmentVariable(GateTelemetry.EnvironmentVariable, null);
        }

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Intent.Metadata", result.Stderr);

        // Other tests may run Cli concurrently while the variable is set, so find this run's line.
        var entry = File.ReadAllLines(log)
            .Select(line => JsonDocument.Parse(line).RootElement)
            .Single(e => e.GetProperty("stdin").GetString() == stdin);
        Assert.Equal("guard-write", entry.GetProperty("command").GetString());
        Assert.Equal(2, entry.GetProperty("exitCode").GetInt32());
        Assert.Equal(result.Stderr, entry.GetProperty("stderr").GetString());
    }

    [Fact]
    public void An_unwritable_log_never_changes_the_verdict()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var unwritable = Path.Combine(repo.Path, "no-such-folder", "gate-telemetry.jsonl");

        GateResult result;
        Environment.SetEnvironmentVariable(GateTelemetry.EnvironmentVariable, unwritable);
        try
        {
            result = GateTestHarness.Run(repo.Path, "{\"tool_input\":{\"file_path\":\"src/ok.cs\"}}", gitChangeProvider: null, "guard-write", "--harness", "codex");
        }
        finally
        {
            Environment.SetEnvironmentVariable(GateTelemetry.EnvironmentVariable, null);
        }

        Assert.Equal(0, result.ExitCode);
        Assert.False(File.Exists(unwritable));
    }
}
