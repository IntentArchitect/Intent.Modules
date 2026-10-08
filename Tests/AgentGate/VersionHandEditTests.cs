using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// A hand-edit of an ".imodspec"'s &lt;version&gt; is held to the same rules as setting the version
/// through the designer - the route agents actually take. Every version change in the eval runs was a
/// hand-edit, so guarding only run_designer_script left the rules unenforced in practice.
/// </summary>
/// <remarks>
/// A valid change is allowed, with a reminder to load the versioning skill; a downgrade stays allowed,
/// because the Software Factory will not write a lower version from the designer.
/// </remarks>
public class VersionHandEditTests
{
    private const string ImodspecRelativePath = "Modules/Sample.Module/Sample.Module.imodspec";

    [Fact]
    public void Allows_a_first_bump_with_a_reminder_to_load_the_versioning_skill()
    {
        var result = Run(head: "1.0.0", current: "1.0.0", edit: "<version>1.0.1-pre.0</version>", scheme: "pre");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("module-version-increment", result.Stdout);
        Assert.Contains("additionalContext", result.Stdout);
        Assert.DoesNotContain("permissionDecision", result.Stdout);
    }

    [Fact]
    public void Denies_a_second_bump_in_the_same_line_of_work()
    {
        var result = Run(head: "1.0.0-pre.8", current: "1.0.0-pre.9", edit: "<version>1.0.0-pre.10</version>", scheme: "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("already moved this line of work", result.Stderr);
        Assert.Contains("module-version-increment", result.Stderr);
    }

    [Fact]
    public void Denies_a_bare_release_version_under_the_prerelease_scheme()
    {
        var result = Run(head: "1.0.0", current: "1.0.0", edit: "<version>1.0.1</version>", scheme: "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("1.0.1-pre.0", result.Stderr);
    }

    [Fact]
    public void Allows_a_bare_release_version_under_the_final_scheme()
    {
        var result = Run(head: "1.0.0", current: "1.0.0", edit: "<version>1.0.1</version>", scheme: "final");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_an_invalid_version()
    {
        var result = Run(head: "1.0.0", current: "1.0.0", edit: "<version>1.0</version>", scheme: "final");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("not a valid semantic version", result.Stderr);
    }

    [Fact]
    public void Allows_a_downgrade_correction_even_after_a_bump()
    {
        // Correcting an over-bump back down is the sanctioned use of the hand-edit.
        var result = Run(head: "1.0.0", current: "1.1.0-pre.0", edit: "<version>1.0.1-pre.0</version>", scheme: "pre");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Stays_silent_when_the_version_does_not_change()
    {
        var result = Run(head: "1.0.0", current: "1.0.0", edit: "<tags>a b</tags>", scheme: "pre");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Checks_a_whole_file_write()
    {
        var result = Run(head: "1.0.0-pre.8", current: "1.0.0-pre.9", edit: null, scheme: "pre", wholeFile: Imodspec("1.0.0-pre.10"));

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Checks_a_codex_apply_patch()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile(ImodspecRelativePath, Imodspec("1.0.0-pre.9"));
        var git = new FakeGitChangeProvider().WithFileAtHead(ImodspecRelativePath, Imodspec("1.0.0-pre.8"));
        var patch = $"*** Begin Patch\n*** Update File: {ImodspecRelativePath}\n@@\n-  <version>1.0.0-pre.9</version>\n+  <version>1.0.0-pre.10</version>\n*** End Patch";
        var stdin = JsonSerializer.Serialize(new { tool_name = "apply_patch", tool_input = new { command = patch } });

        var result = GateTestHarness.Run(repo.Path, stdin, git, "guard-write", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Writes_the_reminder_as_plain_text_for_kiro()
    {
        var result = Run(head: "1.0.0", current: "1.0.0", edit: "<version>1.0.1-pre.0</version>", scheme: "pre", harness: "kiro");

        Assert.Equal(0, result.ExitCode);
        Assert.StartsWith("intent-agent-gate:", result.Stdout.Trim());
    }

    private static GateResult Run(string head, string current, string? edit, string scheme, string? wholeFile = null, string harness = "codex")
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var path = repo.CreateFile(ImodspecRelativePath, Imodspec(current));
        var git = new FakeGitChangeProvider().WithFileAtHead(ImodspecRelativePath, Imodspec(head));
        var toolInput = new Dictionary<string, object?> { ["file_path"] = path };
        if (edit is not null)
        {
            toolInput["new_string"] = edit;
        }

        if (wholeFile is not null)
        {
            toolInput["content"] = wholeFile;
        }

        var stdin = JsonSerializer.Serialize(new Dictionary<string, object?> { ["tool_name"] = "Edit", ["tool_input"] = toolInput });
        return GateTestHarness.Run(repo.Path, stdin, git, "guard-write", "--harness", harness, "--scheme", scheme);
    }

    private static string Imodspec(string version) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>Sample.Module</id>
          <version>{version}</version>
          <summary>Sample summary</summary>
          <description>Sample summary</description>
          <tags>sample tags</tags>
        </package>
        """;
}
