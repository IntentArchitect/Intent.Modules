using Intent.Agent.Gate;

namespace AgentGate.Tests.TestSupport;

public sealed record GateResult(int ExitCode, string Stdout, string Stderr);

public static class GateTestHarness
{
    public static GateResult Run(string workingDirectory, string stdin, IGitChangeProvider? gitChangeProvider, params string[] args)
    {
        using var stdinReader = new StringReader(stdin);
        var stdoutWriter = new StringWriter();
        var stderrWriter = new StringWriter();

        var exitCode = Cli.Run(args, stdinReader, stdoutWriter, stderrWriter, workingDirectory, gitChangeProvider);

        return new GateResult(exitCode, stdoutWriter.ToString(), stderrWriter.ToString());
    }
}
