using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// A partial ".imodspec" edit is judged by whether it changes "&lt;summary&gt;" or "&lt;description&gt;",
/// not by whether either merely appears in it. A version edit that carried the unchanged
/// "&lt;summary&gt;" line as context used to be denied with a message about the summary.
/// </summary>
public class ImodspecSnippetEditTests
{
    [Fact]
    public void Allows_a_version_edit_that_carries_the_unchanged_summary_as_context()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, Edit(imodspec,
            oldString: "<version>1.0.0</version>\n  <summary>Sample summary</summary>",
            newString: "<version>1.0.1</version>\n  <summary>Sample summary</summary>"));

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("module-version-increment", result.Stdout);
    }

    [Fact]
    public void Allows_unchanged_summary_context_when_the_tool_sends_no_replaced_text()
    {
        // Compared against the file on disk instead.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, Edit(imodspec, oldString: null, newString: "<summary>Sample summary</summary>\n  <tags>more tags</tags>"));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_an_edit_that_removes_the_summary()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, Edit(imodspec, oldString: "  <summary>Sample summary</summary>\n", newString: ""));

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Application Settings page", result.Stderr);
    }

    [Fact]
    public void Denies_an_edit_that_changes_the_description_alongside_the_version()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, Edit(imodspec,
            oldString: "<version>1.0.0</version>\n  <summary>Sample summary</summary>\n  <description>Sample summary</description>",
            newString: "<version>1.0.1</version>\n  <summary>Sample summary</summary>\n  <description>Longer description</description>"));

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Codex_apply_patch_allows_a_summary_line_it_removes_and_adds_back_unchanged()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = "apply_patch",
            ["tool_input"] = new Dictionary<string, object?>
            {
                ["command"] = """
                    *** Begin Patch
                    *** Update File: Sample/Sample.imodspec
                    @@
                    -  <version>1.0.0</version>
                    -  <summary>Sample summary</summary>
                    +  <version>1.0.1</version>
                    +  <summary>Sample summary</summary>
                    *** End Patch
                    """,
            },
        }));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_an_edit_that_changes_the_summary_text_without_its_tags()
    {
        // The tool only needs a unique substring, so the tags need not be in the edit at all.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Generates sample widgets for apps."));

        var result = Run(repo, Edit(imodspec, oldString: "Generates sample widgets for apps.</summary>", newString: "Hand-written summary.</summary>"));

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Application Settings page", result.Stderr);
    }

    [Fact]
    public void Denies_a_replace_all_edit_that_changes_the_description_text()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, Edit(imodspec, oldString: "Sample summary", newString: "Other words", replaceAll: true));

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Allows_a_tag_less_version_edit()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspec = repo.CreateFile("Sample/Sample.imodspec", Imodspec("1.0.0", "Sample summary"));

        var result = Run(repo, Edit(imodspec, oldString: "1.0.0</version>", newString: "1.0.1</version>"));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Codex_apply_patch_denies_a_change_to_the_middle_of_a_multi_line_summary()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("Sample/Sample.imodspec", """
            <?xml version="1.0" encoding="utf-8"?>
            <package>
              <id>Sample</id>
              <version>1.0.0</version>
              <summary>
                First line.
                Second line.
              </summary>
            </package>
            """);

        // Codex's own hook runs the gate as "codex"; "claude" would let the step-aside check apply.
        var result = Run(repo, harness: "codex", stdin: JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = "apply_patch",
            ["tool_input"] = new Dictionary<string, object?>
            {
                ["command"] = """
                    *** Begin Patch
                    *** Update File: Sample/Sample.imodspec
                    @@
                       <summary>
                         First line.
                    -    Second line.
                    +    A different second line.
                       </summary>
                    *** End Patch
                    """,
            },
        }));

        Assert.Equal(2, result.ExitCode);
    }

    private static GateResult Run(TempDirectory repo, string stdin, string harness = "claude") =>
        GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", harness, "--scheme", "final");

    private static string Edit(string filePath, string? oldString, string newString, bool replaceAll = false)
    {
        var toolInput = new Dictionary<string, object?> { ["file_path"] = filePath, ["new_string"] = newString };
        if (oldString is not null)
        {
            toolInput["old_string"] = oldString;
        }

        if (replaceAll)
        {
            toolInput["replace_all"] = true;
        }

        return JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = "Edit",
            ["transcript_path"] = "/tmp/transcript.jsonl",
            ["tool_input"] = toolInput,
        });
    }

    private static string Imodspec(string version, string summary) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>Sample</id>
          <version>{version}</version>
          <summary>{summary}</summary>
          <description>{summary}</description>
          <tags>sample</tags>
        </package>
        """;
}
