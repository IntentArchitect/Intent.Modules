using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// The `cases-guard-version` table from the plan: current version, proposed version, configured
/// scheme, one case per row - including the two rows that fail a naive implementation (numeric
/// pre-release comparison, and promotion-off-prerelease being legal under the scheme guard).
/// </summary>
public class GuardVersionTests
{
    private const string ApplicationId = "a1111111-1111-1111-1111-111111111111";

    [Fact]
    public void Allows_a_plain_prerelease_increment_silently()
    {
        var result = Run(currentVersion: "1.0.0-pre.8", proposedVersion: "1.0.0-pre.9", scheme: "pre");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Allows_pre_10_over_pre_9_numeric_not_lexical_comparison()
    {
        var result = Run(currentVersion: "1.0.0-pre.9", proposedVersion: "1.0.0-pre.10", scheme: "pre");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_a_downgrade()
    {
        var result = Run(currentVersion: "1.1.0", proposedVersion: "1.0.3-pre.0", scheme: "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("does not sort strictly higher", result.Stderr);
    }

    [Fact]
    public void Allows_promotion_from_prerelease_to_release_of_the_same_core_version()
    {
        // The scheme guard's own trap: this must not be treated as "missing the -pre suffix".
        var result = Run(currentVersion: "1.0.0-pre.9", proposedVersion: "1.0.0", scheme: "pre");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_a_bare_release_version_when_prerelease_scheme_is_configured()
    {
        var result = Run(currentVersion: "1.0.0", proposedVersion: "1.0.1", scheme: "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("1.0.1-pre.0", result.Stderr);
    }

    [Fact]
    public void Uses_the_generated_default_scheme_when_no_scheme_flag_is_passed()
    {
        // No harness's hook command passes --scheme; the default is baked into the generated Cli.cs
        // from "Use Pre-release Versions", which is switched on in the ModuleBuilderSkills test app
        // these sources are generated into.
        var result = Run(currentVersion: "1.0.0", proposedVersion: "1.0.1", scheme: null);

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("1.0.1-pre.0", result.Stderr);
    }

    [Fact]
    public void Allows_a_bare_release_version_under_final_scheme()
    {
        var result = Run(currentVersion: "1.0.0", proposedVersion: "1.0.1", scheme: "final");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Denies_an_identical_version_as_not_strictly_higher()
    {
        var result = Run(currentVersion: "1.0.0-pre.9", proposedVersion: "1.0.0-pre.9", scheme: "final");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Denies_a_double_bump_when_the_version_already_moved_vs_head()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var imodspecPath = "Modules/Sample.Module/Sample.Module.imodspec";
        repo.CreateFile(imodspecPath, Imodspec("1.0.0-pre.9"));
        repo.CreateFile("Modules/Sample.Module/Sample.Module.application.config", ApplicationConfig());

        var gitProvider = new FakeGitChangeProvider().WithFileAtHead(imodspecPath, Imodspec("1.0.0-pre.8"));
        var stdin = DesignerScriptInput("1.0.0-pre.10");

        var result = GateTestHarness.Run(repo.Path, stdin, gitProvider, "guard-version", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("already moved this line of work", result.Stderr);
    }

    [Fact]
    public void Resolves_a_module_that_does_not_live_under_a_Modules_folder()
    {
        // Module resolution must not assume any particular top-level folder name - a consumer
        // laid out differently (here, "packages/" instead of "Modules/") still has its version
        // checked correctly.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("packages/Sample.Module/Sample.Module.imodspec", Imodspec("1.1.0"));
        repo.CreateFile("packages/Sample.Module/Sample.Module.application.config", ApplicationConfig());

        var stdin = DesignerScriptInput("1.0.3-pre.0");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-version", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("does not sort strictly higher", result.Stderr);
    }

    [Fact]
    public void Resolves_a_module_whose_imodspec_is_not_beside_its_application_config()
    {
        // A solution whose workspace root differs from its metadata folder (e.g. one scaffolded
        // via the Application Template Builder) splits ".application.config" from the
        // application's real generated output - "location" must be followed to find the real
        // ".imodspec", not assumed to sit in the same folder as the config.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("metadata/Sample.Module/Sample.Module.application.config", ApplicationConfig(location: "../../output/Sample.Module"));
        repo.CreateFile("output/Sample.Module/Sample.Module.imodspec", Imodspec("1.1.0"));

        var stdin = DesignerScriptInput("1.0.3-pre.0");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-version", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("does not sort strictly higher", result.Stderr);
    }

    [Fact]
    public void Denies_a_downgrade_outside_a_git_repository()
    {
        // No ".git" anywhere above, and the real git process provider: git fails, so module
        // discovery must fall back to walking the filesystem from the folder the hook runs in.
        using var repo = new TempDirectory();
        Assert.Null(Intent.Agent.Gate.GitRepoLocator.FindRepoRoot(repo.Path));
        repo.CreateFile("metadata/Sample.Module/Sample.Module.application.config", ApplicationConfig(location: "../../output/Sample.Module"));
        repo.CreateFile("output/Sample.Module/Sample.Module.imodspec", Imodspec("1.1.0"));

        var stdin = DesignerScriptInput("1.0.3-pre.0");
        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-version", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("does not sort strictly higher", result.Stderr);
    }

    [Fact]
    public void Allows_silently_and_writes_nothing_when_the_script_does_not_set_version()
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        var stdin = JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["tool_input"] = new Dictionary<string, object?>
            {
                ["applicationId"] = ApplicationId,
                ["script"] = "pkg.ensureStereotype(\"Module Settings\").setProperty(\"API Namespace\", \"Foo\");",
            },
        });

        var result = GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-version", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    private static GateResult Run(string currentVersion, string proposedVersion, string? scheme)
    {
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", Imodspec(currentVersion));
        repo.CreateFile("Modules/Sample.Module/Sample.Module.application.config", ApplicationConfig());

        var stdin = DesignerScriptInput(proposedVersion);

        return scheme is null
            ? GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-version", "--harness", "codex")
            : GateTestHarness.Run(repo.Path, stdin, gitChangeProvider: null, "guard-version", "--harness", "codex", "--scheme", scheme);
    }

    private static string DesignerScriptInput(string proposedVersion) => JsonSerializer.Serialize(new Dictionary<string, object?>
    {
        ["tool_input"] = new Dictionary<string, object?>
        {
            ["applicationId"] = ApplicationId,
            ["script"] = $"pkg.ensureStereotype(\"Module Settings\").setProperty(\"Version\", \"{proposedVersion}\");",
        },
    });

    private static string ApplicationConfig(string location = ".") => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <application id="{ApplicationId}" name="Sample.Module" version="1.0.0" location="{location}" metadataNamingConvention="use-element-name">
        </application>
        """;

    private static string Imodspec(string version) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <package>
          <id>Sample.Module</id>
          <version>{version}</version>
        </package>
        """;
}
