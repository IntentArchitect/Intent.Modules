using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AgentEvals;

/// <summary>What one harness run is given: where to work, what to do, and whether the Intent MCP stub is attached.</summary>
public sealed record RunRequest(Workspace Workspace, string Prompt, string? Model, bool WithIntentStub);

/// <summary>How the suite starts one AI harness as a test subject, and reads back what it did.</summary>
public interface IHarness
{
    /// <summary>Short id used on the command line and in results, e.g. "claude".</summary>
    string Id { get; }

    string DisplayName { get; }

    /// <summary>The gate copy this harness's hooks run, relative to the project root.</summary>
    string GatePath { get; }

    /// <summary>What the modules generate for this harness, copied into each workspace from the test app.</summary>
    IReadOnlyList<string> GeneratedFiles { get; }

    /// <summary>
    /// Whether a guarded action is blocked when the gate itself cannot run. True wherever the hook
    /// config can fail closed; Kiro, for one, has no way to and lets the action through.
    /// </summary>
    bool FailsClosedWhenGateCannotRun { get; }

    string? Executable { get; }

    /// <summary>Why this harness cannot be run right now - not installed, not signed in - or null when it can.</summary>
    Task<string?> NotReadyAsync();

    Task<string> VersionAsync();

    ProcessStartInfo CreateRun(RunRequest request);

    /// <summary>
    /// Why a finished run never really started because the harness could not authenticate, or null.
    /// A sign-in can be present yet no longer valid, which only the run itself reveals; such a run says
    /// nothing about the hooks, so it is reported as a failed run rather than scored.
    /// </summary>
    string? SignInFailure(ProcessResult process);

    Transcript ParseTranscript(string stdout);
}

public static class Harnesses
{
    public static IReadOnlyList<IHarness> Create(EvalPaths paths) =>
    [
        new ClaudeCodeHarness(paths),
        new CodexHarness(paths),
        new OpenCodeHarness(paths),
        new CopilotHarness(paths),
    ];

    internal static async Task<string> VersionOf(string executable, string workingDirectory, params string[] args) =>
        // First line only: some CLIs (Copilot) append an update notice.
        (await Processes.RunAsync(Processes.Command(executable, workingDirectory, args), null, TimeSpan.FromSeconds(30))).Stdout.Trim().Split('\n')[0].Trim();
}

/// <summary>
/// Claude Code, headless, on the subscription sign-in. Only the workspace's own project settings are
/// loaded and only the MCP servers the run names, so the developer's user-level hooks and MCP servers
/// never take part in a run.
/// </summary>
public sealed class ClaudeCodeHarness(EvalPaths paths) : IHarness
{
    public string Id => "claude";

    public string DisplayName => "Claude Code";

    public string GatePath => ".claude/hooks/gate/gate.cs";

    public IReadOnlyList<string> GeneratedFiles { get; } = [".claude/settings.json", ".claude/hooks", ".claude/rules", ".claude/skills"];

    public bool FailsClosedWhenGateCannotRun => true;

    public string? Executable { get; } = Processes.Find("claude");

    public Task<string?> NotReadyAsync() => Task.FromResult(Executable is null ? "not installed" : null);

    public Task<string> VersionAsync() => Harnesses.VersionOf(Executable!, paths.Root, "--version");

    public ProcessStartInfo CreateRun(RunRequest request)
    {
        List<string> args =
        [
            "-p", request.Prompt,
            "--output-format", "stream-json",
            "--verbose",
            "--model", request.Model ?? "haiku",
            "--dangerously-skip-permissions",
            "--setting-sources", "project,local",
            "--no-session-persistence",
            "--strict-mcp-config",
        ];

        if (request.WithIntentStub)
        {
            var (command, stubArgs) = McpStub.LaunchCommand(request.Workspace.Root, request.Workspace.StubLogPath);
            var config = Path.Combine(request.Workspace.RunDirectory, "claude-mcp.json");
            File.WriteAllText(config, JsonSerializer.Serialize(new
            {
                mcpServers = new Dictionary<string, object>
                {
                    [McpStub.ServerName] = new { type = "stdio", command, args = stubArgs },
                },
            }));
            args.AddRange(["--mcp-config", config]);
        }

        var startInfo = Processes.Command(Executable!, request.Workspace.Root, [.. args]);
        // Otherwise Claude Code records an (empty) auto-memory folder per workspace under ~/.claude/projects,
        // even with --no-session-persistence: a write outside the eval root.
        startInfo.Environment["CLAUDE_CODE_DISABLE_AUTO_MEMORY"] = "1";
        return startInfo;
    }

    // Not detected yet: Claude Code's wording for a lapsed sign-in has not been observed.
    public string? SignInFailure(ProcessResult process) => null;

    public Transcript ParseTranscript(string stdout) => TranscriptParsers.ClaudeStreamJson(stdout);
}

/// <summary>
/// Codex, headless. --dangerously-bypass-hook-trust lets the workspace's project hooks run without an
/// interactive trust prompt. --dangerously-bypass-approvals-and-sandbox matches the unrestricted mode
/// every other harness runs in, so a difference between harnesses is the agent's or the hooks', not
/// Codex's own sandbox.
/// </summary>
/// <remarks>
/// Codex runs under its own CODEX_HOME in the eval root's homes folder, signed in there once with the
/// developer's OpenAI API key. The developer's ~/.codex cannot simply be ignored: under
/// --ignore-user-config (codex-cli 0.155) Codex loads no project hooks at all, so the gate never runs.
/// Nor can it be used: every run records its workspace as a trusted project in config.toml, with no
/// option to trust a folder without saving it, and it brings the developer's MCP servers - possibly a
/// live Intent Architect - into a test. The home's config.toml is the runner's, rewritten before every
/// run, which also clears the trust entries earlier runs recorded.
/// </remarks>
public sealed class CodexHarness(EvalPaths paths) : IHarness
{
    private string Home => Path.Combine(paths.Homes, "codex");

    /// <summary>
    /// Signs the Codex home in with the API key the developer's own Codex already uses, piped across
    /// so it never lands in shell history. An API key, unlike a ChatGPT sign-in, does not refresh
    /// itself, so the copy cannot invalidate the original.
    /// </summary>
    private string SignInCommand => OperatingSystem.IsWindows()
        ? $"$env:CODEX_HOME = '{Home}'; (Get-Content ~/.codex/auth.json | ConvertFrom-Json).OPENAI_API_KEY | codex login --with-api-key; Remove-Item Env:CODEX_HOME"
        : $"jq -r .OPENAI_API_KEY ~/.codex/auth.json | CODEX_HOME='{Home}' codex login --with-api-key";

    public string Id => "codex";

    public string DisplayName => "Codex";

    public string GatePath => ".codex/hooks/gate/gate.cs";

    public IReadOnlyList<string> GeneratedFiles { get; } = [".codex/hooks.json", ".codex/hooks", ".agents"];

    public bool FailsClosedWhenGateCannotRun => true;

    public string? Executable { get; } = Processes.Find("codex");

    public async Task<string?> NotReadyAsync()
    {
        if (Executable is null)
        {
            return "not installed";
        }

        Directory.CreateDirectory(Home);
        var status = await Processes.RunAsync(WithHome(Processes.Command(Executable, Home, "login", "status")), null, TimeSpan.FromSeconds(30));
        return status.ExitCode == 0 ? null : $"no Codex sign-in in the eval root yet. Sign in once with: {SignInCommand}";
    }

    public Task<string> VersionAsync() => Harnesses.VersionOf(Executable!, paths.Root, "--version");

    public ProcessStartInfo CreateRun(RunRequest request)
    {
        var config = """
            # Owned by the agent eval suite and rewritten before every run: no MCP servers, plugins, hooks
            # or model overrides beyond what the run names, so a run depends on nothing but the workspace
            # and the command line. Trust entries Codex records here are cleared each run.

            """;
        if (request.WithIntentStub)
        {
            var (command, stubArgs) = McpStub.LaunchCommand(request.Workspace.Root, request.Workspace.StubLogPath);
            config += $"""

                [mcp_servers.{McpStub.ServerName}]
                command = {Toml(command)}
                args = [{string.Join(", ", stubArgs.Select(Toml))}]

                """;
        }

        File.WriteAllText(Path.Combine(Home, "config.toml"), config);

        var args = new List<string>
        {
            "exec", "--json", "--dangerously-bypass-hook-trust", "--ephemeral",
            "--dangerously-bypass-approvals-and-sandbox", "--skip-git-repo-check",
        };
        if (request.Model is not null)
        {
            args.AddRange(["-m", request.Model]);
        }

        args.Add(request.Prompt);
        return WithHome(Processes.Command(Executable!, request.Workspace.Root, [.. args]));
    }

    public Transcript ParseTranscript(string stdout) => TranscriptParsers.CodexJson(stdout);

    /// <remarks>
    /// Observed with codex-cli 0.155: a missing or invalid credential surfaces only as error events -
    /// "unexpected status 401 Unauthorized ... auth error code: invalid_api_key" - ending in turn.failed,
    /// while "codex login status" still reports the stored credential as signed in, e.g. a revoked key.
    /// "refresh token" and "sign/log in again" cover a lapsed ChatGPT sign-in, should one be used.
    /// Only error lines are searched, so the agent quoting such text in its own work cannot match.
    /// </remarks>
    public string? SignInFailure(ProcessResult process)
    {
        var failed = (process.Stdout + "\n" + process.Stderr)
            .Split('\n')
            .Where(line => line.Contains("\"type\":\"error\"") || line.Contains("\"type\":\"turn.failed\"") || !line.TrimStart().StartsWith('{'))
            .Any(line => SignInError.IsMatch(line));
        return failed ? $"Codex could not authenticate, so the run tested nothing. Sign the eval root in again with: {SignInCommand}" : null;
    }

    private static readonly Regex SignInError = new(
        @"401 Unauthorized|auth error|refresh token|(sign|log) ?in again|not logged in",
        RegexOptions.IgnoreCase);

    /// <summary>A TOML literal string: no escapes, so Windows paths survive as they are.</summary>
    private static string Toml(string value) => $"'{value}'";

    private ProcessStartInfo WithHome(ProcessStartInfo startInfo)
    {
        startInfo.Environment["CODEX_HOME"] = Home;
        return startInfo;
    }
}
