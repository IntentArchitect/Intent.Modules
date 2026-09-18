using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// What guard-write protects: Intent Architect's own METADATA - the designer model, an application's
/// configuration, and the record of what it generated and installed.
/// </summary>
/// <remarks>
/// An earlier version protected the files LISTED in managed-files.xml, which was a misreading of
/// "managed-files.xml must not be touched": that meant the manifest itself, not its contents. The
/// difference is not academic - in real use the old scope produced false positive after false
/// positive and not one true positive, because authoring a scaffolded template, writing release
/// notes and correcting a csproj package version are all intended workflows. Several tests here
/// assert the inversion directly, so the misreading cannot return quietly.
/// <para>
/// Three rows (module icon overwrite) remain <see cref="Assert.Skip"/> - that check was never
/// implemented, and the plan's "coverage-honesty" principle applies to the suite as much as to the
/// shipped behaviour.
/// </para>
/// </remarks>
public class GuardWriteTests
{
    [Fact]
    public void Denies_editing_an_applications_managed_files_manifest()
    {
        // managed-files.xml is Intent's own record of what it generated. Note what this test does NOT
        // say: the files LISTED inside it are not protected. That misreading is what the guard used to
        // enforce, and it produced only false positives - authoring a scaffolded template, writing
        // release notes and correcting a csproj package version are all intended workflows.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var manifest = repo.CreateFile("Modules/Sample.Module/Sample.Module.application.managed-files.xml", "<files />");

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: manifest, content: "<files />"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Intent MCP", result.Stdout + result.Stderr);
    }

    [Fact]
    public void Denies_editing_an_application_config()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var config = repo.CreateFile("Modules/Sample.Module/Sample.Module.application.config", "<application />");

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: config, content: "<application />"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Denies_editing_designer_model_xml_under_Intent_Metadata()
    {
        // The designer model. Changed through run_designer_script, never by editing the XML.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var element = repo.CreateFile("Modules/Sample.Module/Intent.Metadata/Module Builder/Elements/Thing__abc123.xml", "<element />");

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: element, content: "<element />"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("run_designer_script", result.Stdout + result.Stderr);
    }

    [Fact]
    public void Denies_editing_anything_under_a_dot_intent_folder()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var file = repo.CreateFile(".intent/something.json", "{}");

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: file, content: "{}"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Allows_editing_generated_output_because_that_is_not_metadata()
    {
        // The inversion, stated outright. A generated skill file is Software-Factory OUTPUT: editing it
        // is usually futile because the next run overwrites it, but it corrupts nothing and it is not
        // this guard's business. Guidance covers it; a hook blocking it only ever got in the way.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var generated = repo.CreateFile("Modules/Sample.Module/.agents/skills/some-skill/SKILL.md", "# Skill");
        repo.CreateFile("Modules/Sample.Module/Sample.Module.application.managed-files.xml", """
            <?xml version="1.0" encoding="utf-8"?>
            <files>
              <file path=".agents/skills/some-skill/SKILL.md" templateId="Sample.Templates.SkillMd" />
            </files>
            """);

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: generated, newString: "# Edited"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_editing_a_scaffolded_template_partial()
    {
        // Hand-authoring the ignored method bodies is the only way to write a template at all. This
        // needed a named exemption under the old scope; under the new one it is simply not metadata.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var templatePath = repo.CreateFile("Modules/Sample.Module/Templates/Thing/ThingTemplatePartial.cs", "// template logic");

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: templatePath, newString: "// authored by hand"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_editing_release_notes_and_the_csproj()
    {
        // Both were denied under the old scope, and both denials blocked real work: release notes are
        // the documentation chore this module mandates, and the repo's own known-build-gotchas says
        // NuGet package versions are corrected in the csproj.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var notes = repo.CreateFile("Modules/Sample.Module/release-notes.md", "### Version 1.0.0");
        var csproj = repo.CreateFile("Modules/Sample.Module/Sample.Module.csproj", "<Project />");

        Assert.Equal(0, GateTestHarness.Run(repo.Path, ToolInput(filePath: notes, newString: "- Fixed: something."), gitChangeProvider: null, "guard-write", "--harness", "codex").ExitCode);
        Assert.Equal(0, GateTestHarness.Run(repo.Path, ToolInput(filePath: csproj, newString: "<PackageReference />"), gitChangeProvider: null, "guard-write", "--harness", "codex").ExitCode);
    }

    [Fact]
    public void Allows_editing_claude_settings()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var settingsPath = repo.CreateFile(".claude/settings.json", "{}");

        var result = GateTestHarness.Run(repo.Path, ToolInput(filePath: settingsPath, newString: "{}"), gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
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
    public void Denies_editing_summary_in_imodspec_via_opencodes_camelcase_newstring_field()
    {
        // OpenCode's own "edit" tool uses "newString" (camelCase), not "new_string" - confirmed by
        // directly probing a real opencode run's tool.execute.before payload, not assumed.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", ImodspecXml(version: "1.0.0"));

        var stdin = """{"tool_input":{"file_path":"REPLACED","newString":"<summary>A different summary</summary>"}}""".Replace("REPLACED", imodspecPath.Replace("\\", "\\\\"));
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-write", "--harness", "codex");

        Assert.Equal(2, result.ExitCode);
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
