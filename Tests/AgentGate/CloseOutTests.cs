using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The `cases-close-out` table from the plan: fixture repository states, one case per row. Two
/// rows are marked <see cref="Assert.Skip"/> - see the comment on each - because that check was
/// never implemented (a specification gap for the .csproj-version row, deliberate scope-limiting
/// for the dependency-audit and icon rows). Close-out never denies, so every assertion here is
/// about the exit code staying 0 and the message content, never about blocking.
/// </summary>
public class CloseOutTests
{
    private const string ModuleName = "Sample.Module";
    private const string ImodspecRelativePath = "Modules/Sample.Module/Sample.Module.imodspec";
    private const string ApplicationConfigRelativePath = "Modules/Sample.Module/Sample.Module.application.config";

    /// <summary>
    /// Module discovery finds a module via its ".application.config" (id + location), not by
    /// scanning for a bare "Modules/{name}/" folder - so every fixture that wants to be found
    /// needs one of these alongside its ".imodspec", exactly as a real module does.
    /// </summary>
    private static string WellFormedApplicationConfig() => """
        <?xml version="1.0" encoding="utf-8"?>
        <application id="11111111-1111-1111-1111-111111111111" name="Sample.Module" location="." />
        """;

    [Fact]
    public void Silent_when_nothing_changed()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var gitProvider = new FakeGitChangeProvider();

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Warns_when_module_changed_but_version_never_moved()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, WellFormedImodspec());
        repo.CreateFile(ApplicationConfigRelativePath, WellFormedApplicationConfig());
        repo.CreateFile("Modules/Sample.Module/docs/README.md", "# Sample.Module\n");
        repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");
        var gitProvider = new FakeGitChangeProvider()
            .WithChangedFiles("Modules/Sample.Module/SomeFile.cs", "Modules/Sample.Module/CONTEXT.md");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("version moving", result.Stdout);
        Assert.Contains(ModuleName, result.Stdout);
    }

    [Fact(Skip = ".imodspec-vs-.csproj version disagreement was not implemented - scaffolded module .csproj files carry no <Version>/<PackageVersion> property to compare against (verified against this module's own .csproj), so the check as named has no concrete signal to read.")]
    public void Warns_when_imodspec_version_and_csproj_version_disagree()
    {
    }

    [Fact(Skip = "Referenced-but-undeclared dependency detection was not implemented - deferred as needing real cross-referencing of .csproj/template imports against .imodspec, a bigger standalone piece than this pass took on.")]
    public void Warns_when_a_template_imports_an_undeclared_dependency()
    {
    }

    [Fact]
    public void Warns_when_release_notes_heading_still_carries_the_pre_suffix()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, WellFormedImodspec(version: "1.0.3-pre.0"));
        repo.CreateFile(ApplicationConfigRelativePath, WellFormedApplicationConfig());
        repo.CreateFile("Modules/Sample.Module/docs/README.md", "# Sample.Module\n");
        repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");
        repo.CreateFile("Modules/Sample.Module/release-notes.md", "### Version 1.0.3-pre.0\n\n- New Feature: something.\n");
        var gitProvider = new FakeGitChangeProvider()
            .WithChangedFiles(ImodspecRelativePath, "Modules/Sample.Module/CONTEXT.md");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("-pre suffix belongs stripped", result.Stdout);
    }

    [Fact]
    public void Warns_when_module_changed_and_context_md_untouched()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, WellFormedImodspec());
        repo.CreateFile(ApplicationConfigRelativePath, WellFormedApplicationConfig());
        repo.CreateFile("Modules/Sample.Module/docs/README.md", "# Sample.Module\n");
        repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");
        var gitProvider = new FakeGitChangeProvider()
            .WithChangedFiles(ImodspecRelativePath, "Modules/Sample.Module/SomeFile.cs");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("CONTEXT.md was not touched", result.Stdout);
    }

    [Fact]
    public void Warns_when_module_has_no_docs_readme()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, WellFormedImodspec());
        repo.CreateFile(ApplicationConfigRelativePath, WellFormedApplicationConfig());
        repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");
        var gitProvider = new FakeGitChangeProvider()
            .WithChangedFiles(ImodspecRelativePath, "Modules/Sample.Module/CONTEXT.md");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("No docs/README.md", result.Stdout);
    }

    [Fact]
    public void Warns_when_tags_are_not_lowercase()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, WellFormedImodspec(tags: "Module Builder"));
        repo.CreateFile(ApplicationConfigRelativePath, WellFormedApplicationConfig());
        repo.CreateFile("Modules/Sample.Module/docs/README.md", "# Sample.Module\n");
        repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");
        var gitProvider = new FakeGitChangeProvider()
            .WithChangedFiles(ImodspecRelativePath, "Modules/Sample.Module/CONTEXT.md");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("not lowercase", result.Stdout);
    }

    [Fact(Skip = "Font-awesome-icon detection was not implemented - deferred, same as the guard-write icon-overwrite rows, for lack of a reliable 'is this a stock/font-awesome icon' signal.")]
    public void Warns_when_module_icon_is_font_awesome()
    {
    }

    [Fact]
    public void Does_not_treat_an_ancestor_application_as_the_module_beneath_it()
    {
        // "Area" is an application whose output root is an ancestor folder holding another
        // application's module one level down. That imodspec belongs to "Child", which has its own
        // .application.config - a change elsewhere under "area/" must not be reported as a change to
        // a module called "area".
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("metadata/Area/Area.application.config", """
            <?xml version="1.0" encoding="utf-8"?>
            <application id="22222222-2222-2222-2222-222222222222" name="Area" location="../../area" />
            """);
        repo.CreateFile("area/Child/Child.application.config", """
            <?xml version="1.0" encoding="utf-8"?>
            <application id="33333333-3333-3333-3333-333333333333" name="Child" location="." />
            """);
        repo.CreateFile("area/Child/Child.imodspec", WellFormedImodspec());
        var gitProvider = new FakeGitChangeProvider().WithChangedFiles("area/Other/SomeTest.cs");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Finishes_silently_outside_a_git_repository()
    {
        // Real git process, no ".git" anywhere above. "git diff HEAD" outside a repository writes a
        // multi-kilobyte usage dump to stderr; if that stream is redirected but never read, the pipe
        // fills and close-out hangs forever, which blocks the agent. Bounded, so a regression fails
        // instead of hanging the suite.
        using var repo = new TempDirectory();
        Assert.Null(Intent.Agent.Gate.GitRepoLocator.FindRepoRoot(repo.Path));

        var run = Task.Run(() => GateTestHarness.Run(repo.Path, string.Empty, gitChangeProvider: null, "close-out", "--harness", "codex"));

        Assert.True(run.Wait(TimeSpan.FromSeconds(30)), "close-out did not finish outside a git repository.");
        Assert.Equal(0, run.Result.ExitCode);
        Assert.Empty(run.Result.Stdout);
    }

    [Fact]
    public void Silent_when_everything_is_in_order()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, WellFormedImodspec(version: "1.0.0"));
        repo.CreateFile(ApplicationConfigRelativePath, WellFormedApplicationConfig());
        repo.CreateFile("Modules/Sample.Module/docs/README.md", "# Sample.Module\n");
        repo.CreateFile("Modules/Sample.Module/CONTEXT.md", "# Context\n");
        repo.CreateFile("Modules/Sample.Module/release-notes.md", "### Version 1.0.0\n\n- New Feature: something.\n");
        var gitProvider = new FakeGitChangeProvider()
            .WithChangedFiles(ImodspecRelativePath, "Modules/Sample.Module/CONTEXT.md", "Modules/Sample.Module/SomeFile.cs");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    private static string WellFormedImodspec(string version = "1.0.0", string tags = "sample eval fixture") => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>{ModuleName}</id>
          <version>{version}</version>
          <tags>{tags}</tags>
        </package>
        """;
}
