using System.Diagnostics;
using AgentGate.Tests.TestSupport;
using Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Proves the file-based app actually runs - the SDK floor and the "Directory.Build.props" shield
/// are invisible to a linked build, which only proves the logic. This is this repository's own
/// "compiling is not working" rule, one level up: a suite that stopped at linking would repeat it.
/// </summary>
/// <remarks>
/// The hook-command tests run the exact strings <see cref="GateCommands"/> generates, in the shell
/// each one is generated for, so a change to a command form is tested where it will run.
/// </remarks>
public class GateSmokeTests
{
    private static readonly string GatePath = RepoRootLocator.FindGateEntryPoint();

    private const string DenyStdin = "{\"tool_input\":{\"file_path\":\"Intent.Metadata/element.xml\"}}";

    [Fact]
    public void Gate_entry_point_exists_where_the_module_generates_it()
    {
        Assert.True(File.Exists(GatePath), $"Expected the generated gate entry point at '{GatePath}'.");
    }

    [Fact]
    public void Gate_is_one_self_contained_file_with_no_include_directives()
    {
        // "#:include" needs .NET SDK 10.0.300+. On 10.0.102 the gate failed to compile with
        // "Unrecognized directive 'include'" and blocked every guarded edit with no explanation, so
        // the module now writes one file that any .NET 10 SDK compiles.
        var files = Directory.GetFiles(Path.GetDirectoryName(GatePath)!).Select(file => Path.GetFileName(file)).Order().ToArray();
        var content = File.ReadAllText(GatePath);

        Assert.Equal(new[] { "Directory.Build.props", "gate.cs" }, files);
        Assert.DoesNotContain("#:include", content);
        Assert.StartsWith("#!", content);
    }

    [Fact]
    public void Dotnet_run_warm_compiles_and_exits_zero()
    {
        var result = Run("dotnet", ["run", GatePath, "--", "warm"], RepoRootLocator.Find());

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Dotnet_run_with_no_arguments_exits_two_with_usage()
    {
        EnsureWarm();
        var result = Run("dotnet", ["run", GatePath, "--no-build", "--"], RepoRootLocator.Find());

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Usage:", result.Stderr);
    }

    [Fact]
    public void Dotnet_run_guard_write_denies_real_intent_metadata_in_this_repo()
    {
        // Targets a real metadata file in the real repo, so the assertion is about the shipped,
        // generated gate rather than a fixture.
        EnsureWarm();
        var repoRoot = RepoRootLocator.Find();
        var realMetadata = Path.Combine(repoRoot, "Tests", "ModuleBuilderSkills", "ModuleBuilderSkills.application.config");
        Assert.True(File.Exists(realMetadata), "Expected a known Intent metadata path to exist for this smoke test to target.");

        var stdin = "{\"tool_input\":{\"file_path\":\"" + realMetadata.Replace("\\", "\\\\") + "\"}}";
        var result = Run("dotnet", ["run", GatePath, "--no-build", "--", "guard-write", "--harness", "codex"], repoRoot, stdin);

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Intent MCP", result.Stderr);
    }

    [Fact]
    public void Dotnet_run_guard_write_allows_real_generated_output_in_this_repo()
    {
        // The counterpart. This file is genuinely Software-Factory output listed in a
        // managed-files.xml, and the gate must allow it: generated output is not metadata.
        EnsureWarm();
        var repoRoot = RepoRootLocator.Find();
        var realGenerated = Path.Combine(repoRoot, "Tests", "ModuleBuilderSkills", ".agents", "instructions", "module-building-workflow.instructions.md");
        Assert.True(File.Exists(realGenerated), "Expected a known generated-output path to exist for this smoke test to target.");

        var stdin = "{\"tool_input\":{\"file_path\":\"" + realGenerated.Replace("\\", "\\\\") + "\"}}";
        var result = Run("dotnet", ["run", GatePath, "--no-build", "--", "guard-write", "--harness", "codex"], repoRoot, stdin);

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void A_build_failure_exits_one_not_two_which_is_why_guard_commands_must_wrap_it()
    {
        // "dotnet run" itself returns 1 on a build failure (syntax error, wrong SDK) - never 2. Most
        // harnesses treat 1 as an ALLOW, so a bare guard command would silently stop protecting
        // anything the moment the gate could not build - which is why the guards are wrapped, and
        // proven separately below. Cli.cs's own try/catch cannot help: it never runs.
        using var project = new TempDirectory();
        var gate = CopyGate(project, ".claude", broken: true);

        var result = Run("dotnet", ["run", gate, "--", "warm"], project.Path);

        Assert.Equal(1, result.ExitCode);
    }

    [Fact]
    public void Posix_guard_blocks_when_the_gate_cannot_build()
    {
        using var project = new TempDirectory();
        CopyGate(project, ".claude", broken: true);

        var result = Run("sh", ["-c", GateCommands.GuardPosix(".claude", "guard-write")], project.Path, "{}");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void PowerShell_guard_blocks_when_the_gate_cannot_build()
    {
        // Codex's commandWindows and Copilot's powershell field both run this form in PowerShell.
        var powerShell = FindPowerShell();
        using var project = new TempDirectory();
        CopyGate(project, ".codex", broken: true);

        var result = Run(powerShell, ["-NoProfile", "-Command", GateCommands.GuardPowerShell(".codex", "guard-write")], project.Path, "{}");

        Assert.Equal(2, result.ExitCode);
    }

    [Fact]
    public void Portable_guard_keeps_a_deny_as_exit_two_in_both_shells()
    {
        // PowerShell hands a native program's exit 2 back as 1, and Kiro let a denied write through
        // on that 1 - observed. "; exit $LASTEXITCODE" restores it in PowerShell and is a no-op in sh.
        var powerShell = FindPowerShell();
        using var project = new TempDirectory();
        var gate = CopyGate(project, ".kiro", broken: false);
        Assert.Equal(0, Run("dotnet", ["run", gate, "--", "warm"], project.Path).ExitCode);
        var command = GateCommands.GuardPortable(".kiro", "guard-write");

        Assert.Equal(2, Run("sh", ["-c", command], project.Path, DenyStdin).ExitCode);
        Assert.Equal(2, Run(powerShell, ["-NoProfile", "-Command", command], project.Path, DenyStdin).ExitCode);
    }

    [Fact]
    public void Close_out_never_exits_two_even_when_the_gate_cannot_build()
    {
        // A Stop hook exiting 2 makes Claude Code and Codex keep the agent going; a broken gate must
        // not trap it in a loop of turns.
        using var project = new TempDirectory();
        CopyGate(project, ".claude", broken: true);

        var result = Run("sh", ["-c", GateCommands.CloseOut(".claude")], project.Path, "{}");

        Assert.NotEqual(2, result.ExitCode);
    }

    [Fact]
    public void Concurrent_invocations_with_no_build_do_not_fail_each_other()
    {
        // The accepted mitigation for build contention: warm once, then every hook invocation passes
        // --no-build, so concurrent hook firings never race on the same build output.
        EnsureWarm();

        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(() => Run("dotnet", ["run", GatePath, "--no-build", "--", "warm"], RepoRootLocator.Find())))
            .ToArray();

        Task.WaitAll(tasks);

        foreach (var task in tasks)
        {
            Assert.Equal(0, task.Result.ExitCode);
        }
    }

    private static void EnsureWarm() => Run("dotnet", ["run", GatePath, "--", "warm"], RepoRootLocator.Find());

    /// <summary>
    /// Copies the generated gate into a throwaway project at "&lt;harnessFolder&gt;/hooks/gate/", the
    /// path <see cref="GateCommands"/> expects relative to the project root. A broken copy fails to
    /// compile, standing in for every way dotnet can fail before the gate's own code runs.
    /// </summary>
    private static string CopyGate(TempDirectory project, string harnessFolder, bool broken)
    {
        var sourceFolder = Path.GetDirectoryName(GatePath)!;
        var content = File.ReadAllText(GatePath) + (broken ? Environment.NewLine + "this is not C#" + Environment.NewLine : "");
        project.CreateFile(Path.Combine(harnessFolder, "hooks", "gate", "Directory.Build.props"),
            File.ReadAllText(Path.Combine(sourceFolder, "Directory.Build.props")));
        return project.CreateFile(Path.Combine(harnessFolder, "hooks", "gate", "gate.cs"), content);
    }

    private static string FindPowerShell()
    {
        foreach (var candidate in OperatingSystem.IsWindows() ? new[] { "powershell", "pwsh" } : ["pwsh"])
        {
            try
            {
                if (Run(candidate, ["-NoProfile", "-Command", "exit 0"], Path.GetTempPath()).ExitCode == 0)
                {
                    return candidate;
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Not installed - try the next one.
            }
        }

        Assert.Skip("No PowerShell is installed, so the PowerShell command forms cannot be exercised here.");
        return null!;
    }

    private static (int ExitCode, string Stdout, string Stderr) Run(string fileName, string[] args, string workingDirectory, string? stdin = null)
    {
        var startInfo = new ProcessStartInfo(fileName)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = stdin is not null,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException($"Failed to start {fileName}.");

        if (stdin is not null)
        {
            process.StandardInput.Write(stdin);
            process.StandardInput.Close();
        }

        // Both streams drained concurrently: a redirected stream left unread can fill its pipe and
        // hang the child, which is the exact failure the gate itself once had with git.
        var stderr = process.StandardError.ReadToEndAsync();
        var stdout = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, stdout, stderr.Result);
    }
}
