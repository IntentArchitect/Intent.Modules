using AgentGate.Tests.TestSupport;
using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// What close-out and module discovery learn from a real git repository. Both list untracked files
/// from one "git status" rather than "git ls-files --others", which in a large repository took long
/// enough for close-out to outrun a 30 second hook timeout; these pin that the cheaper call still
/// sees the same files.
/// </summary>
public class GitChangeDetectionTests
{
    [Fact]
    public void Changed_files_cover_edits_deletions_staged_and_untracked_files_but_not_ignored_ones()
    {
        using var repo = GitRepository.Init();
        repo.CreateFile(".gitignore", "bin/\n");
        repo.CreateFile("Mod/Edited.cs", "class A {}");
        repo.CreateFile("Mod/Deleted.cs", "class B {}");
        repo.Commit("Mod/Unchanged.cs", "class C {}", "base");

        repo.CreateFile("Mod/Edited.cs", "class A { int X; }");
        File.Delete(repo.FullPath("Mod/Deleted.cs"));
        repo.CreateFile("Mod/Staged.cs", "class D {}");
        repo.Git("add", "--", "Mod/Staged.cs");
        repo.CreateFile("New Module/Sub/Brand new é.cs", "class E {}");
        repo.CreateFile("Mod/bin/Output.dll", "binary");

        var changed = new ProcessGitChangeProvider().GetChangedFiles(repo.Path);

        Assert.Equal(
            ["Mod/Deleted.cs", "Mod/Edited.cs", "Mod/Staged.cs", "New Module/Sub/Brand new é.cs"],
            changed.Order(StringComparer.Ordinal));
    }

    [Fact]
    public void Discovery_finds_tracked_and_untracked_application_configs_but_not_ignored_ones()
    {
        using var repo = GitRepository.Init();
        repo.CreateFile(".gitignore", "bin/\n");
        repo.Commit("Tracked/Tracked.application.config", ApplicationConfig("tracked"), "base");
        repo.CreateFile("Brand New/Deep/New.application.config", ApplicationConfig("untracked"));
        repo.CreateFile("Brand New/bin/Copied.application.config", ApplicationConfig("ignored"));
        repo.CreateFile("Brand New/Deep/New.application.config.bak", ApplicationConfig("not-a-config"));

        var ids = ModuleDiscovery.FindAll(repo.Path).Select(application => application.ApplicationId);

        Assert.Equal(["tracked", "untracked"], ids.Order(StringComparer.Ordinal));
    }

    private static string ApplicationConfig(string id) => $"<application id=\"{id}\" location=\".\" />";
}
