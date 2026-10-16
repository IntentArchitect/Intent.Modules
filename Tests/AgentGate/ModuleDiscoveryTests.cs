using System.Text.Json;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Which application a file, or an applicationId, belongs to - the module discovery close-out and
/// guard-version share. Both fixed cases here were found in a real session on Linux, where
/// close-out warned about a module called "Modules".
/// </summary>
public class ModuleDiscoveryTests
{
    private const string ModuleId = "a1111111-1111-1111-1111-111111111111";
    private const string ParentId = "b2222222-2222-2222-2222-222222222222";

    [Fact]
    public void Close_out_follows_a_location_written_with_backslashes()
    {
        // Intent writes "location" with Windows separators. On Linux a backslash is just a file-name
        // character, so the module's output root resolved to a folder that does not exist and the
        // change was attributed to the ancestor application instead, under that application's name.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("intent/Sample.Module/Sample.Module.application.config", ApplicationConfig(ModuleId, @"..\..\Sample.Module"));
        repo.CreateFile("intent/Dogfood/Dogfood.application.config", ApplicationConfig(ParentId, "../.."));
        repo.CreateFile("Sample.Module/Sample.Module.imodspec", Imodspec("1.0.0"));
        var gitProvider = new FakeGitChangeProvider().WithChangedFiles("Sample.Module/SomeTemplate.cs");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("version moving: Sample.Module.", result.Stdout);
        Assert.DoesNotContain(Path.GetFileName(repo.Path), result.Stdout);
    }

    [Fact]
    public void Guard_version_follows_a_location_written_with_backslashes()
    {
        // The same misreading made guard-version find no ".imodspec" and allow the call: it failed open.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("metadata/Sample.Module/Sample.Module.application.config", ApplicationConfig(ModuleId, @"..\..\output\Sample.Module"));
        repo.CreateFile("output/Sample.Module/Sample.Module.imodspec", Imodspec("1.1.0"));

        var result = GateTestHarness.Run(repo.Path, DesignerScriptInput(ModuleId, "1.0.3-pre.0"), gitChangeProvider: null,
            "guard-version", "--harness", "codex", "--scheme", "pre");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("does not sort strictly higher", result.Stderr);
    }

    [Fact]
    public void Close_out_does_not_treat_an_application_whose_output_root_holds_other_modules_as_one()
    {
        // An application rooted at "Modules/" whose module applications keep their configs elsewhere
        // (here "intent/"). Only folders holding an ".application.config" used to be skipped, so the
        // parent claimed the first module's ".imodspec" and warned about a module called "Modules".
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("intent/Parent/Parent.application.config", ApplicationConfig(ParentId, "../../Modules"));
        repo.CreateFile("intent/Sample.Module/Sample.Module.application.config", ApplicationConfig(ModuleId, "../../Modules/Sample.Module"));
        repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", Imodspec("1.0.0"));
        var gitProvider = new FakeGitChangeProvider().WithChangedFiles("Modules/Directory.Build.props");

        var result = GateTestHarness.Run(repo.Path, string.Empty, gitProvider, "close-out", "--harness", "codex");

        Assert.Equal(0, result.ExitCode);
        Assert.Empty(result.Stdout);
    }

    [Fact]
    public void Guard_version_does_not_attribute_another_applications_module_to_its_parent()
    {
        // Setting the parent's version must not be checked against the module beneath it.
        using var repo = new TempDirectory();
        repo.MarkAsRepoRoot();
        repo.CreateFile("intent/Parent/Parent.application.config", ApplicationConfig(ParentId, "../../Modules"));
        repo.CreateFile("intent/Sample.Module/Sample.Module.application.config", ApplicationConfig(ModuleId, "../../Modules/Sample.Module"));
        repo.CreateFile("Modules/Sample.Module/Sample.Module.imodspec", Imodspec("2.0.0"));

        var result = GateTestHarness.Run(repo.Path, DesignerScriptInput(ParentId, "1.0.0"), gitChangeProvider: null,
            "guard-version", "--harness", "codex", "--scheme", "final");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("could not locate the module's .imodspec", result.Stderr);
    }

    private static string DesignerScriptInput(string applicationId, string proposedVersion) => JsonSerializer.Serialize(new Dictionary<string, object?>
    {
        ["tool_input"] = new Dictionary<string, object?>
        {
            ["applicationId"] = applicationId,
            ["script"] = $"pkg.ensureStereotype(\"Module Settings\").setProperty(\"Version\", \"{proposedVersion}\");",
        },
    });

    private static string ApplicationConfig(string id, string location) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <application id="{id}" name="App" version="1.0.0" location="{location}">
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
