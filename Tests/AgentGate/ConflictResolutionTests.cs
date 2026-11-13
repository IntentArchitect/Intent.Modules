using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Resolving a merge or rebase conflict in a file the guard protects. Both cases were hit on a real
/// master-to-development rebase: a conflicted ".imodspec" was denied with the summary message
/// whatever the edit changed, and a conflicted ".pkg.config" under Intent.Metadata had no route at
/// all, so the agent finished the rebase with shell commands the gate never saw.
/// </summary>
/// <remarks>
/// The unmerged cases build a real conflict with git, because "is this path unmerged" is answered
/// by git itself and a fake would prove nothing about that.
/// </remarks>
public class ConflictResolutionTests
{
    private const string ImodspecPath = "Modules/Sample.Module/Sample.Module.imodspec";
    private const string PkgConfigPath = "Modules/Sample.Module/Intent.Metadata/Module Builder/Sample.Module/Sample.Module.pkg.config";

    [Fact]
    public void Allows_resolving_a_dependencies_conflict_in_an_imodspec_with_an_edit()
    {
        // The file on disk has conflict markers, so it is not XML. The field check used to read that
        // as "no summary", compare it with the summary in the resolved file, and deny.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var path = repo.CreateFile(ImodspecPath, ConflictedDependenciesImodspec);

        var result = Run(repo.Path, EditInput(path, DependenciesConflictBlock, "    <dependency id=\"Intent.Common\" version=\"3.11.9\" />\n"));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_resolving_a_dependencies_conflict_in_an_imodspec_with_a_whole_file_write()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var path = repo.CreateFile(ImodspecPath, ConflictedDependenciesImodspec);

        var result = Run(repo.Path, WriteInput(path, Imodspec("Sample summary", "3.11.9")));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Still_denies_a_summary_change_made_while_resolving_an_imodspec_conflict()
    {
        // Outside git's unmerged state, the summary rule holds for a conflicted file as for any other.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var path = repo.CreateFile(ImodspecPath, ConflictedDependenciesImodspec);

        var result = Run(repo.Path, WriteInput(path, Imodspec("A new summary", "3.11.9")));

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("<summary> and <description>", result.Stdout + result.Stderr);
    }

    [Fact]
    public void Allows_resolving_an_unmerged_metadata_file_with_a_whole_file_write()
    {
        using var repo = ConflictRepo.Create(PkgConfigPath, PkgConfig("1.0.0"), PkgConfig("1.0.1"), PkgConfig("1.0.1-pre.1"));

        var result = Run(repo.Path, WriteInput(repo.FullPath(PkgConfigPath), PkgConfig("1.0.1-pre.1")));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Allows_resolving_an_unmerged_metadata_file_with_an_edit()
    {
        using var repo = ConflictRepo.Create(PkgConfigPath, PkgConfig("1.0.0"), PkgConfig("1.0.1"), PkgConfig("1.0.1-pre.1"));
        var onDisk = File.ReadAllText(repo.FullPath(PkgConfigPath)).Replace("\r\n", "\n");
        var conflictBlock = onDisk[onDisk.IndexOf("<<<<<<<", StringComparison.Ordinal)..(onDisk.IndexOf("\n", onDisk.IndexOf(">>>>>>>", StringComparison.Ordinal), StringComparison.Ordinal) + 1)];

        var result = Run(repo.Path, EditInput(repo.FullPath(PkgConfigPath), conflictBlock, "  <Version>1.0.1-pre.1</Version>\n"));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_a_resolution_that_leaves_conflict_markers_in_an_unmerged_metadata_file()
    {
        using var repo = ConflictRepo.Create(PkgConfigPath, PkgConfig("1.0.0"), PkgConfig("1.0.1"), PkgConfig("1.0.1-pre.1"));
        var stillConflicted = File.ReadAllText(repo.FullPath(PkgConfigPath)) + "\n";

        var result = Run(repo.Path, WriteInput(repo.FullPath(PkgConfigPath), stillConflicted));

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("conflict markers", result.Stdout + result.Stderr);
    }

    [Fact]
    public void Allows_taking_the_other_sides_summary_while_an_imodspec_is_unmerged()
    {
        // The summary itself conflicts. Either side is a legitimate resolution - the Software Factory
        // rewrites it from Application Settings anyway - so the field rule waits until git has the
        // file merged.
        using var repo = ConflictRepo.Create(ImodspecPath,
            Imodspec("Base summary", "3.11.0"), Imodspec("Ours summary", "3.11.0"), Imodspec("Theirs summary", "3.11.0"));

        var result = Run(repo.Path, WriteInput(repo.FullPath(ImodspecPath), Imodspec("Theirs summary", "3.11.0")));

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_editing_a_metadata_file_again_once_its_conflict_is_staged()
    {
        // The allowance lasts only while git lists the path as unmerged.
        using var repo = ConflictRepo.Create(PkgConfigPath, PkgConfig("1.0.0"), PkgConfig("1.0.1"), PkgConfig("1.0.1-pre.1"));
        File.WriteAllText(repo.FullPath(PkgConfigPath), PkgConfig("1.0.1-pre.1"));
        repo.Git("add", "--", PkgConfigPath);

        var result = Run(repo.Path, WriteInput(repo.FullPath(PkgConfigPath), PkgConfig("1.0.2")));

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Intent.Metadata", result.Stdout + result.Stderr);
    }

    [Fact]
    public void Allows_a_codex_patch_resolving_an_unmerged_metadata_file()
    {
        using var repo = ConflictRepo.Create(PkgConfigPath, PkgConfig("1.0.0"), PkgConfig("1.0.1"), PkgConfig("1.0.1-pre.1"));
        var lines = File.ReadAllText(repo.FullPath(PkgConfigPath)).Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
        var start = Array.FindIndex(lines, l => l.StartsWith("<<<<<<<", StringComparison.Ordinal));
        var end = Array.FindIndex(lines, l => l.StartsWith(">>>>>>>", StringComparison.Ordinal));
        var removed = lines[start..(end + 1)].Select(l => "-" + l);
        var patch = string.Join("\n", new[] { "*** Begin Patch", $"*** Update File: {PkgConfigPath}", "@@", " " + lines[start - 1] }
            .Concat(removed)
            .Append("+  <Version>1.0.1-pre.1</Version>")
            .Append("*** End Patch"));

        var result = Run(repo.Path, JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = "apply_patch",
            ["tool_input"] = new Dictionary<string, object?> { ["command"] = patch },
        }), "codex");

        Assert.Equal(0, result.ExitCode);
    }

    private static GateResult Run(string repoPath, string stdin, string harness = "claude") =>
        GateTestHarness.Run(repoPath, stdin, gitChangeProvider: null, "guard-write", "--harness", harness);

    private static string EditInput(string filePath, string oldString, string newString) =>
        JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = "Edit",
            ["tool_input"] = new Dictionary<string, object?> { ["file_path"] = filePath, ["old_string"] = oldString, ["new_string"] = newString },
        });

    private static string WriteInput(string filePath, string content) =>
        JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_name"] = "Write",
            ["tool_input"] = new Dictionary<string, object?> { ["file_path"] = filePath, ["content"] = content },
        });

    private static string Imodspec(string summary, string commonVersion) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>Sample.Module</id>
          <version>1.0.1-pre.0</version>
          <summary>{summary}</summary>
          <description>{summary}</description>
          <dependencies>
            <dependency id="Intent.Common" version="{commonVersion}" />
          </dependencies>
        </package>

        """.Replace("\r\n", "\n");

    private static string PkgConfig(string version) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <Version>{version}</Version>
        </package>

        """.Replace("\r\n", "\n");

    private const string DependenciesConflictBlock =
        "<<<<<<< HEAD\n" +
        "    <dependency id=\"Intent.Common\" version=\"3.11.9\" />\n" +
        "=======\n" +
        "    <dependency id=\"Intent.Common\" version=\"3.11.5-pre.0\" />\n" +
        ">>>>>>> feature\n";

    private static readonly string ConflictedDependenciesImodspec =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
        "<package>\n" +
        "  <id>Sample.Module</id>\n" +
        "  <version>1.0.1-pre.0</version>\n" +
        "  <summary>Sample summary</summary>\n" +
        "  <description>Sample summary</description>\n" +
        "  <dependencies>\n" +
        DependenciesConflictBlock +
        "  </dependencies>\n" +
        "</package>\n";

    /// <summary>A real git repository stopped on a merge conflict in one file.</summary>
    private static class ConflictRepo
    {
        public static GitRepository Create(string relativePath, string baseContent, string oursContent, string theirsContent)
        {
            var repo = GitRepository.Init();
            repo.Commit(relativePath, baseContent, "base");
            repo.Git("checkout", "-q", "-b", "feature");
            repo.Commit(relativePath, theirsContent, "feature");
            repo.Git("checkout", "-q", "main");
            repo.Commit(relativePath, oursContent, "ours");
            repo.Git(allowFailure: true, "merge", "feature");
            Assert.NotEqual(string.Empty, repo.Git("ls-files", "-u", "--", relativePath).Trim());
            return repo;
        }
    }
}
