using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Kiro CLI v3 (2.28, headless, Windows) runs hook commands in cmd.exe and sends its str_replace edits
/// as "newStr" - both observed in a real run, where the gate let a summary edit through.
/// </summary>
public class KiroPayloadTests
{
    [Fact]
    public void Drops_the_powershell_tail_that_cmd_passes_through_as_arguments()
    {
        var args = Cli.WithoutShellTail(["guard-write", "--harness", "kiro;", "exit", "$LASTEXITCODE"]);

        Assert.Equal(["guard-write", "--harness", "kiro"], args);
    }

    [Fact]
    public void Leaves_arguments_without_a_shell_tail_alone()
    {
        string[] original = ["guard-write", "--harness", "kiro"];

        Assert.Equal(original, Cli.WithoutShellTail(original));
    }

    [Fact]
    public void Denies_a_summary_edit_sent_as_newStr_and_answers_in_kiros_shape()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", "<package><version>1.0.0</version><summary>Old</summary></package>");
        var stdin = JsonSerializer.Serialize(new
        {
            hook_event_name = "PreToolUse",
            tool_name = "str_replace",
            tool_input = new { path = imodspec, oldStr = "<summary>Old</summary>", newStr = "<summary>New</summary>", replace_all = false },
        });

        var result = GateTestHarness.Run(repo.Path, stdin, null, "guard-write", "--harness", "kiro;", "exit", "$LASTEXITCODE");

        Assert.Equal(2, result.ExitCode);
        // Kiro reads the reason from stderr and nothing from stdout on a deny.
        Assert.Empty(result.Stdout);
        Assert.Contains("<summary>", result.Stderr);
    }
}
