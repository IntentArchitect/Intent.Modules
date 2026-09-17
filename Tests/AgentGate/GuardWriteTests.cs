using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The `cases-guard-write` table from the plan: path and edit content, one case per row. Three
/// rows (module icon overwrite) are marked <see cref="Assert.Skip"/> - see the comment on each -
/// because that check was never implemented; the plan's own "coverage-honesty" principle applies
/// to the test suite too, not just the shipped behaviour.
/// </summary>
public class GuardWriteTests
{
    [Fact]
    public void Denies_a_path_listed_in_managed_files_xml_naming_the_owning_template()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var generatedPath = repo.CreateFile("Modules/Sample.Module/Generated.cs", "// generated");
        repo.CreateFile("Modules/Sample.Module/Sample.Module.application.managed-files.xml", """
            <?xml version="1.0" encoding="utf-8"?>
            <files>
              <file path="Generated.cs" templateId="Sample.Module.Templates.GeneratedTemplate" />
            </files>
            """);

        var stdin = ToolInput(filePath: generatedPath, newString: "// hand-edited");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Sample.Module.Templates.GeneratedTemplate", result.Stdout);
        Assert.Contains("Never edit generated output", result.Stderr);
    }

    [Fact]
    public void Denies_modules_config_unconditionally()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var modulesConfigPath = repo.CreateFile("Modules/SomeApp/modules.config", "<modules></modules>");

        var stdin = ToolInput(filePath: modulesConfigPath, newString: "<modules><module moduleId=\"X\" version=\"1.0.0\" /></modules>");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("modules.config", result.Stderr);
    }

    [Fact]
    public void Allows_editing_tags_in_imodspec()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0", tags: "old tags"));

        var stdin = ToolInput(filePath: imodspecPath, newString: "<tags>new tags</tags>");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_adding_a_dependency_in_imodspec()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0"));

        var stdin = ToolInput(filePath: imodspecPath, newString: "<dependency id=\"Intent.Common\" version=\"3.11.4\" />");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_lowering_the_version_in_imodspec_the_sanctioned_downgrade_route()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.1.0"));

        var stdin = ToolInput(filePath: imodspecPath, newString: "<version>1.0.3-pre.0</version>");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_editing_summary_in_imodspec()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0"));

        var stdin = ToolInput(filePath: imodspecPath, newString: "<summary>A different summary</summary>");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Application Settings page", result.Stderr);
    }

    [Fact]
    public void Denies_editing_description_in_imodspec()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0"));

        var stdin = ToolInput(filePath: imodspecPath, newString: "<description>A different description</description>");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Allows_a_whole_file_write_that_changes_only_tags()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0", tags: "old tags"));
        var wholeFile = ImodspecXml(version: "1.0.0", tags: "new tags updated");

        var stdin = ToolInput(filePath: imodspecPath, content: wholeFile);
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_a_whole_file_write_that_changes_summary_even_though_it_is_not_a_partial_edit()
    {
        // This is the trap the plan's "imodspec-scope" callout exists for: a guard that inspects
        // only the edit string passes every Edit case and then silently allows a <summary> change
        // delivered as a full-file write - exactly how an agent that just read the file delivers it.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0", summary: "Original summary"));
        var wholeFile = ImodspecXml(version: "1.0.0", summary: "A completely different summary");

        var stdin = ToolInput(filePath: imodspecPath, content: wholeFile);
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Denies_a_managed_path_when_the_owning_apps_output_root_is_not_its_metadata_folder()
    {
        // A real, previously-unverified bug: for an app whose ".application.config" has
        // location=".." (its metadata sits in a subfolder while its actual output lands
        // elsewhere - e.g. a dogfooding app outputting to the repo root), managed-files.xml
        // entries are relative to that OUTPUT root, not to the folder the xml file itself sits
        // in. Resolving relative to the xml's own directory alone silently allows a genuinely
        // managed path through.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var generatedPath = repo.CreateFile("SomeApp/OutputRoot/generated.md", "generated content");
        repo.CreateFile("Modules/DogfoodApp/DogfoodApp.application.config", """
            <?xml version="1.0" encoding="utf-8"?>
            <application id="11111111-1111-1111-1111-111111111111" name="DogfoodApp" version="1.0.0" location="..\..\SomeApp\OutputRoot" metadataNamingConvention="use-element-name">
            </application>
            """);
        repo.CreateFile("Modules/DogfoodApp/DogfoodApp.application.managed-files.xml", """
            <?xml version="1.0" encoding="utf-8"?>
            <files>
              <file path="generated.md" templateId="Some.Module.Templates.GeneratedMd" />
            </files>
            """);

        var stdin = ToolInput(filePath: generatedPath, newString: "hand-edited content");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Some.Module.Templates.GeneratedMd", result.Stdout);
    }

    [Fact]
    public void Allows_editing_context_md()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var contextPath = repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");

        var stdin = ToolInput(filePath: contextPath, newString: "# Context\n\n## New decision\n");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_editing_docs_readme()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var readmePath = repo.CreateFile("Modules/Sample.Module/docs/README.md", "# Sample.Module\n");

        var stdin = ToolInput(filePath: readmePath, newString: "# Sample.Module\n\nUpdated section.\n");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact(Skip = "Icon-overwrite deny was not implemented - detecting 'is this a stock icon' had no reliable specification. See the plan's coverage-honesty callout.")]
    public void Allows_setting_an_icon_when_none_present()
    {
    }

    [Fact(Skip = "Icon-overwrite deny was not implemented - detecting 'is this a stock icon' had no reliable specification. See the plan's coverage-honesty callout.")]
    public void Allows_overwriting_a_stock_icon()
    {
    }

    [Fact(Skip = "Icon-overwrite deny was not implemented - detecting 'is this a stock icon' had no reliable specification. See the plan's coverage-honesty callout.")]
    public void Denies_overwriting_a_custom_icon_unless_the_user_asked()
    {
    }

    private static string ToolInput(string filePath, string? newString = null, string? content = null)
    {
        var toolInput = new Dictionary<string, object?> { ["file_path"] = filePath };
        if (newString is not null)
        {
            toolInput["new_string"] = newString;
        }

        if (content is not null)
        {
            toolInput["content"] = content;
        }

        return JsonSerializer.Serialize(new Dictionary<string, object?> { ["tool_input"] = toolInput });
    }

    private static string ImodspecXml(string version, string tags = "sample tags", string summary = "Sample summary") => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>Sample.Module</id>
          <version>{version}</version>
          <summary>{summary}</summary>
          <description>{summary}</description>
          <tags>{tags}</tags>
        </package>
        """;
}
