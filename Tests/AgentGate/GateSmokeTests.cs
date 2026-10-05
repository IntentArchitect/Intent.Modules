using System.Diagnostics;
using AgentGate.Tests.TestSupport;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Proves the file-based app actually runs - "#:include" resolution, the SDK floor, and the
/// "Directory.Build.props" shield are all invisible to a linked build, which only proves the
/// logic. This is this repository's own "compiling is not working" rule, one level up: a suite
/// that stopped at linking would repeat it.
/// </summary>
public class GateSmokeTests
{
    private static readonly string GatePath = RepoRootLocator.FindGateEntryPoint();

    [Fact]
    public void Gate_entry_point_exists_where_the_module_generates_it()
    {
        Assert.True(File.Exists(GatePath), $"Expected the generated gate entry point at '{GatePath}'.");
    }

    [Fact]
    public void Dotnet_run_warm_compiles_and_exits_zero()
    {
        var result = RunDotnet(["run", GatePath, "--", "warm"]);

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Dotnet_run_with_no_arguments_exits_two_with_usage()
    {
        var result = RunDotnet(["run", GatePath, "--no-build", "--", ]);

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Usage:", result.Stderr);
    }

    [Fact]
    public void Dotnet_run_guard_write_denies_real_intent_metadata_in_this_repo()
    {
        // Targets a real metadata file in the real repo, so the assertion is about the shipped,
        // generated gate rather than a fixture.
        var repoRoot = RepoRootLocator.Find();
        var realMetadata = Path.Combine(repoRoot, "Tests", "ModuleBuilderSkills", "ModuleBuilderSkills.application.config");
        Assert.True(File.Exists(realMetadata), "Expected a known Intent metadata path to exist for this smoke test to target.");

        var escapedPath = realMetadata.Replace("\\", "\\\\");
        var stdin = "{\"tool_input\":{\"file_path\":\"" + escapedPath + "\"}}";
        var result = RunDotnet(["run", GatePath, "--no-build", "--", "guard-write", "--harness", "codex"], stdin);

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Intent MCP", result.Stderr);
    }

    [Fact]
    public void Dotnet_run_guard_write_allows_real_generated_output_in_this_repo()
    {
        // The counterpart, and the one that would have caught the original misreading. This file is
        // genuinely Software-Factory output listed in a managed-files.xml, and the gate must allow it:
        // generated output is not metadata. Editing it is usually futile rather than harmful, and
        // blocking it produced nothing but false positives in real use.
        var repoRoot = RepoRootLocator.Find();
        var realGenerated = Path.Combine(repoRoot, "Tests", "ModuleBuilderSkills", ".agents", "instructions", "module-building-workflow.instructions.md");
        Assert.True(File.Exists(realGenerated), "Expected a known generated-output path to exist for this smoke test to target.");

        var escapedPath = realGenerated.Replace("\\", "\\\\");
        var stdin = "{\"tool_input\":{\"file_path\":\"" + escapedPath + "\"}}";
        var result = RunDotnet(["run", GatePath, "--no-build", "--", "guard-write", "--harness", "codex"], stdin);

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void A_build_failure_exits_one_not_two_which_is_why_hook_commands_must_wrap_it()
    {
        // "dotnet run" itself returns 1 on a build failure (missing #:include, syntax error, wrong
        // SDK) - never 2. Cursor's own docs confirm any exit code other than exactly 0 or 2 is
        // treated as an ALLOW, so a bare "dotnet run gate.cs -- <cmd>" hook command would silently
        // stop protecting anything the moment the gate's own source broke. This is why every
        // generated hook command is suffixed with a shell wrapper that collapses any non-zero exit
        // into exit 2 - proven separately below - rather than relying on Cli.cs's own try/catch,
        // which cannot run until after the build already succeeded.
        var hiddenPath = GatePath.Replace("gate.cs", "SemVer.cs");
        var tempPath = hiddenPath + ".hidden-for-test";
        File.Move(hiddenPath, tempPath);
        try
        {
            var result = RunDotnet(["run", GatePath, "--", "warm"]);

            Assert.Equal(1, result.ExitCode);
        }
        finally
        {
            File.Move(tempPath, hiddenPath);
        }
    }

    [Fact]
    public void The_shell_wrapper_pattern_converts_a_build_failure_into_exit_two()
    {
        var hiddenPath = GatePath.Replace("gate.cs", "SemVer.cs");
        var tempPath = hiddenPath + ".hidden-for-test";
        File.Move(hiddenPath, tempPath);
        try
        {
            // The exact wrapper suffix every generated hook command carries, per
            // HooksJsonTemplatePartial.cs / CLAUDE_SETUP.md.
            var startInfo = new ProcessStartInfo("sh")
            {
                WorkingDirectory = RepoRootLocator.Find(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            startInfo.ArgumentList.Add("-c");
            startInfo.ArgumentList.Add($"dotnet run \"{GatePath}\" -- warm; test $? -eq 0 && exit 0 || exit 2");

            using var process = Process.Start(startInfo)!;
            process.StandardOutput.ReadToEnd();
            process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.Equal(2, process.ExitCode);
        }
        finally
        {
            File.Move(tempPath, hiddenPath);
        }
    }

    [Fact]
    public void Concurrent_invocations_with_no_build_do_not_fail_each_other()
    {
        // The plan's accepted mitigation for build contention: warm once, then every hook
        // invocation passes --no-build, so concurrent hook firings never race on the same build
        // output. This proves that path is actually contention-free, not just documented as one.
        RunDotnet(["run", GatePath, "--", "warm"]); // ensure a build exists before going concurrent

        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(() => RunDotnet(["run", GatePath, "--no-build", "--", "warm"])))
            .ToArray();

        Task.WaitAll(tasks);

        foreach (var task in tasks)
        {
            Assert.Equal(0, task.Result.ExitCode);
        }
    }

    private static (int ExitCode, string Stdout, string Stderr) RunDotnet(string[] args, string? stdin = null)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = RepoRootLocator.Find(),
            RedirectStandardInput = stdin is not null,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start dotnet process.");

        if (stdin is not null)
        {
            process.StandardInput.Write(stdin);
            process.StandardInput.Close();
        }

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, stdout, stderr);
    }
}
