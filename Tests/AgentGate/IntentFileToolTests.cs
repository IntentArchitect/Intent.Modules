using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Intent Architect's own MCP file tools - write_file, patch_file and delete_code_file - write as
/// surely as a harness's edit tool, so guard-write applies the same rules to them. Their payloads use
/// "filePath", "content" and "searchBlock"/"replaceBlock", and a relative path resolves against the
/// Intent workspace (the ".isln" folder), not the git root.
/// </summary>
public class IntentFileToolTests
{
    [Fact]
    public void Denies_write_file_on_intent_metadata()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();

        var result = Run(repo, "mcp__intent-architect__write_file",
            new { filePath = Path.Combine(repo.Path, "App", "App.application.config"), content = "<application />" });

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("application metadata", result.Stderr);
    }

    [Fact]
    public void Denies_delete_code_file_on_modules_config()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var modulesConfig = repo.CreateFile("App/modules.config", "<modules />");

        var result = Run(repo, "mcp__intent-architect__delete_code_file", new { filePath = modulesConfig });

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Denies_write_file_that_changes_the_imodspec_summary()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Old summary"));

        var result = Run(repo, "mcp__intent-architect__write_file", new { filePath = imodspec, content = Imodspec("1.0.0", "New summary") });

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Application Settings page", result.Stderr);
    }

    [Fact]
    public void Patch_file_version_edit_is_held_to_the_version_rules()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var bare = Run(repo, "mcp__intent-architect__patch_file",
            new { filePath = imodspec, searchBlock = "<version>1.0.0</version>", replaceBlock = "<version>1.0.1</version>" }, scheme: "pre");
        var valid = Run(repo, "mcp__intent-architect__patch_file",
            new { filePath = imodspec, searchBlock = "<version>1.0.0</version>", replaceBlock = "<version>1.0.1-pre.0</version>" }, scheme: "pre");

        Assert.Equal(2, bare.ExitCode);
        Assert.Contains("1.0.1-pre.0", bare.Stderr);
        Assert.Equal(0, valid.ExitCode);
        Assert.Contains("module-version-increment", valid.Stdout);
    }

    [Fact]
    public void Resolves_a_relative_path_against_the_intent_solution_folder()
    {
        // The git root is the repository; the Intent workspace is the folder holding the ".isln".
        // Resolved against the git root, "Sample/Sample.imodspec" does not exist and the version rules
        // had nothing to compare against.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("intent/Solution.isln", "<solution />");
        repo.CreateFile("intent/Sample/Sample.imodspec", Imodspec("2.0.0", "Sample summary"));

        var result = Run(repo, "mcp__intent-architect__patch_file",
            new { filePath = "Sample/Sample.imodspec", searchBlock = "<version>2.0.0</version>", replaceBlock = "<version>not-a-version</version>" });

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("not a valid semantic version", result.Stderr);
    }

    [Fact]
    public void Allows_write_file_on_ordinary_generated_output()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();

        var result = Run(repo, "mcp__intent-architect__write_file",
            new { filePath = Path.Combine(repo.Path, "Sample", "Templates", "MyTemplatePartial.cs"), content = "// code" });

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    private static GateResult Run(TempDirectory repo, string toolName, object toolInput, string scheme = "final") =>
        GateTestHarness.Run(repo.Path, JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = toolName,
            ["transcript_path"] = "/tmp/transcript.jsonl",
            ["tool_input"] = toolInput,
        }), gitChangeProvider: null, "guard-write", "--harness", "claude", "--scheme", scheme);

    private static string Imodspec(string version, string summary) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>Sample</id>
          <version>{version}</version>
          <summary>{summary}</summary>
          <description>{summary}</description>
        </package>
        """;
}
