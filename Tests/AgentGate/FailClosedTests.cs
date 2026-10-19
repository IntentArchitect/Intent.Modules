using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The plan's "fail-closed" design decision: if a guard throws, it must block (exit 2), via an
/// EXPLICIT exit(2) - never the runtime's default unhandled-exception code, which Cursor's own docs
/// confirm is treated as an allow for anything other than exactly 0 or 2. close-out is the one
/// exception: it never blocks, crash or not.
/// </summary>
public class FailClosedTests
{
    private sealed class ThrowingGitChangeProvider : IGitChangeProvider
    {
        public IReadOnlyList<string> GetChangedFiles(string repoRoot) => throw new InvalidOperationException("simulated crash");

        public string? TryReadFileAtHead(string repoRoot, string relativePath) => throw new InvalidOperationException("simulated crash");
    }

    [Fact]
    public void Close_out_lets_the_turn_end_when_the_git_provider_throws()
    {
        // A Stop hook exiting 2 tells Claude Code and Codex the agent must KEEP GOING. A close-out
        // that blocked on its own crash trapped the agent in a loop of turns it could do nothing
        // about - observed in real use. It reports the skip instead and allows the turn to end.
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

        Assert.Equal(0, exitCode);
        Assert.Contains("close-out checks were skipped", stderr.ToString());
    }

    [Fact]
    public void A_guard_still_blocks_when_the_git_provider_throws()
    {
        // The counterpart: the close-out exception must not have loosened the guards. guard-version
        // reads HEAD through the git provider, so a crash there has to stay a deny.
        using var repo = new AgentGate.Tests.TestSupport.TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("Mod/Mod.application.config", "<application id=\"app-1\" location=\".\" />");
        repo.CreateFile("Mod/Mod.imodspec", "<package><id>Mod</id><version>1.0.0</version></package>");
        var stdin = "{\"tool_input\":{\"applicationId\":\"app-1\",\"script\":\"pkg.ensureStereotype('Module Settings').setProperty('Version', '1.0.1')\"}}";

        using var reader = new StringReader(stdin);
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var exitCode = Cli.Run(["guard-version", "--harness", "codex"], reader, stdout, stderr, repo.Path, new ThrowingGitChangeProvider());

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
