using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The plan's "fail-closed" design decision: if the gate itself throws, it must block (exit 2),
/// via an EXPLICIT exit(2) - never the runtime's default unhandled-exception code, which Cursor's
/// own docs confirm is treated as an allow for anything other than exactly 0 or 2.
/// </summary>
public class FailClosedTests
{
    private sealed class ThrowingGitChangeProvider : IGitChangeProvider
    {
        public IReadOnlyList<string> GetChangedFiles(string repoRoot) => throw new InvalidOperationException("simulated crash");

        public string? TryReadFileAtHead(string repoRoot, string relativePath) => throw new InvalidOperationException("simulated crash");
    }

    [Fact]
    public void Close_out_blocks_rather_than_crashing_when_the_git_provider_throws()
    {
        using var stdin = new StringReader(string.Empty);
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var exitCode = Cli.Run(
            ["close-out", "--harness", "codex"],
            stdin,
            stdout,
            stderr,
            Directory.GetCurrentDirectory(),
            new ThrowingGitChangeProvider());

        Assert.Equal(2, exitCode);
        Assert.Contains("unexpected error", stderr.ToString());
    }

    [Fact]
    public void An_unknown_command_blocks_rather_than_silently_no_opping()
    {
        using var stdin = new StringReader(string.Empty);
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var exitCode = Cli.Run(["not-a-real-command"], stdin, stdout, stderr, Directory.GetCurrentDirectory());

        Assert.Equal(2, exitCode);
    }

    [Fact]
    public void No_arguments_blocks_with_usage_rather_than_defaulting_to_allow()
    {
        using var stdin = new StringReader(string.Empty);
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var exitCode = Cli.Run([], stdin, stdout, stderr, Directory.GetCurrentDirectory());

        Assert.Equal(2, exitCode);
        Assert.Contains("Usage:", stderr.ToString());
    }
}
