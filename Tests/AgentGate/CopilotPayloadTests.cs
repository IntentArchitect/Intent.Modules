using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Copilot CLI's own hook files send camelCase "toolName"/"toolArgs" - observed as an object, and
/// documented as a JSON-encoded string - with its own tool names ("edit", "create", "view") and
/// argument names ("path", "new_str", "file_text"). Copilot's hook is registered for "guard-tool",
/// which routes designer scripts to the version guard and everything else to the write guard.
/// </summary>
public class CopilotPayloadTests
{
    private const string ApplicationId = "a1111111-1111-1111-1111-111111111111";

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Denies_a_summary_edit_whether_toolArgs_is_an_object_or_a_string(bool argsAsString)
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", "<package><version>1.0.0</version><summary>Old</summary></package>");

        var args = new { path = imodspec, old_str = "<summary>Old</summary>", new_str = "<summary>New</summary>" };
        var result = GateTestHarness.Run(repo.Path, Payload("edit", args, argsAsString), null, "guard-tool", "--harness", "github");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("\"permissionDecision\":\"deny\"", result.Stdout);
    }

    [Fact]
    public void Denies_creating_a_file_inside_Intent_Metadata()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();

        var args = new { path = Path.Combine(repo.Path, "App", "Intent.Metadata", "Element.xml"), file_text = "<element />" };
        var result = GateTestHarness.Run(repo.Path, Payload("create", args), null, "guard-tool", "--harness", "github");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Allows_viewing_metadata_silently()
    {
        // A read carries a path too. Copilot used to send every tool call to guard-write, so a view
        // of an .application.config would have been denied like an edit.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var config = repo.CreateFile("App/App.application.config", "<application />");

        var result = GateTestHarness.Run(repo.Path, Payload("view", new { path = config }), null, "guard-tool", "--harness", "github");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Writes_nothing_to_stdout_for_a_tool_without_a_path()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();

        var result = GateTestHarness.Run(repo.Path, Payload("powershell", new { command = "dotnet build" }), null, "guard-tool", "--harness", "github");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Routes_a_designer_script_to_the_version_guard()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("Modules/Sample.Module/Sample.Module.application.config", $"""<application id="{ApplicationId}" location="." />""");
        repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", "<package><id>Sample.Module</id><version>1.0.0</version></package>");

        var args = new { applicationId = ApplicationId, script = "pkg.ensureStereotype(\"Module Settings\").setProperty(\"Version\", \"1.0.1\")" };
        var result = GateTestHarness.Run(repo.Path, Payload("intent-architect-run_designer_script", args), null, "guard-tool", "--harness", "github", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("1.0.1-pre.0", result.Stderr);
    }

    private static string Payload(string toolName, object toolArgs, bool argsAsString = false) => JsonSerializer.Serialize(new
    {
        sessionId = "s",
        timestamp = 1791391461402,
        cwd = ".",
        toolName,
        toolArgs = argsAsString ? (object)JsonSerializer.Serialize(toolArgs) : toolArgs,
    });
}
