using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Cursor's beforeMCPExecution payload carries "tool_input" as a JSON-ENCODED STRING, per Cursor's hook
/// reference, rather than an object. Read as a string, the guards found no path and no script and
/// allowed every MCP call.
/// </summary>
public class CursorPayloadTests
{
    private const string ApplicationId = "a1111111-1111-1111-1111-111111111111";

    [Fact]
    public void Guard_write_denies_an_intent_file_tool_writing_metadata()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var config = Path.Combine(repo.Path, "App", "App.application.config");

        var result = GateTestHarness.Run(repo.Path, CursorMcpPayload("write_file", new { filePath = config, content = "<application />" }),
            gitChangeProvider: null, "guard-write", "--harness", "cursor");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("deny", result.Stdout);
    }

    [Fact]
    public void Guard_version_reads_a_designer_script_from_a_string_tool_input()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("Sample/Sample.application.config", $"""<application id="{ApplicationId}" name="Sample" location="." />""");
        repo.CreateFile("Sample/Sample.imodspec", "<package><id>Sample</id><version>1.1.0</version></package>");

        var result = GateTestHarness.Run(repo.Path, CursorMcpPayload("run_designer_script", new
        {
            applicationId = ApplicationId,
            script = "pkg.ensureStereotype(\"Module Settings\").setProperty(\"Version\", \"1.0.0\");",
        }), gitChangeProvider: null, "guard-version", "--harness", "cursor", "--scheme", "final");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("does not sort strictly higher", result.Stderr);
    }

    [Fact]
    public void Guard_write_allows_an_unrelated_mcp_write()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();

        var result = GateTestHarness.Run(repo.Path,
            CursorMcpPayload("write_file", new { filePath = Path.Combine(repo.Path, "src", "Thing.cs"), content = "// code" }),
            gitChangeProvider: null, "guard-write", "--harness", "cursor");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    /// <summary>The documented beforeMCPExecution shape: common fields, the tool name, and the input as a string.</summary>
    private static string CursorMcpPayload(string toolName, object arguments) => JsonSerializer.Serialize(new Dictionary<string, object?>
    {
        ["hook_event_name"] = "beforeMCPExecution",
        ["conversation_id"] = "c1",
        ["generation_id"] = "g1",
        ["workspace_roots"] = new[] { "/repo" },
        ["tool_name"] = toolName,
        ["tool_input"] = JsonSerializer.Serialize(arguments),
        ["command"] = "intent-architect",
    });
}
