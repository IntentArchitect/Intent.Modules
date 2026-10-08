using System.Diagnostics;
using System.Text.Json;

namespace AgentEvals;

/// <summary>
/// OpenCode, headless (`opencode run --format json --auto`). Its config, data, cache and state all
/// follow the XDG folders, which are pointed into the eval root's homes folder, so the developer's own
/// OpenCode config, plugins and MCP servers take no part and nothing is written to their profile.
/// </summary>
/// <remarks>
/// OpenCode needs a model provider. It runs on the OpenRouter API key the developer's own OpenCode is
/// signed in with, read at run time and passed as OPENROUTER_API_KEY - never copied into the eval root.
/// The runner's own config (the Intent MCP stub, when attached) is passed through OPENCODE_CONFIG, so
/// nothing extra lands in the workspace.
/// </remarks>
public sealed class OpenCodeHarness(EvalPaths paths) : IHarness
{
    private string Home => Path.Combine(paths.Homes, "opencode");

    private static string UserAuthFile =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "opencode", "auth.json");

    public string Id => "opencode";

    public string DisplayName => "OpenCode";

    public string GatePath => ".opencode/hooks/gate/gate.cs";

    public IReadOnlyList<string> GeneratedFiles { get; } = [".opencode", ".agents"];

    public bool FailsClosedWhenGateCannotRun => true;

    public string? Executable { get; } = Processes.Find("opencode");

    public Task<string?> NotReadyAsync() => Task.FromResult(
        Executable is null ? "not installed"
        : OpenRouterKey() is null ? $"no OpenRouter API key in {UserAuthFile}. Add one with: opencode auth login"
        : null);

    public Task<string> VersionAsync() => Harnesses.VersionOf(Executable!, paths.Root, "--version");

    public ProcessStartInfo CreateRun(RunRequest request)
    {
        var config = new Dictionary<string, object> { ["$schema"] = "https://opencode.ai/config.json" };
        if (request.WithIntentStub)
        {
            var (command, stubArgs) = McpStub.LaunchCommand(request.Workspace.Root, request.Workspace.StubLogPath);
            config["mcp"] = new Dictionary<string, object>
            {
                [McpStub.ServerName] = new { type = "local", command = new[] { command }.Concat(stubArgs).ToArray(), enabled = true },
            };
        }

        var configPath = Path.Combine(request.Workspace.RunDirectory, "opencode.json");
        File.WriteAllText(configPath, JsonSerializer.Serialize(config));

        var startInfo = Processes.Command(Executable!, request.Workspace.Root,
            "run", "--format", "json", "--auto", "--dir", request.Workspace.Root,
            "-m", request.Model ?? "openrouter/anthropic/claude-haiku-4.5", request.Prompt);
        foreach (var (name, folder) in new[] { ("XDG_CONFIG_HOME", "config"), ("XDG_DATA_HOME", "data"), ("XDG_CACHE_HOME", "cache"), ("XDG_STATE_HOME", "state") })
        {
            startInfo.Environment[name] = Path.Combine(Home, folder);
        }

        startInfo.Environment["OPENCODE_CONFIG"] = configPath;
        startInfo.Environment["OPENROUTER_API_KEY"] = OpenRouterKey();
        return startInfo;
    }

    public string? SignInFailure(ProcessResult process) =>
        process.Stderr.Contains("401", StringComparison.Ordinal) && process.Stderr.Contains("auth", StringComparison.OrdinalIgnoreCase)
            ? "OpenCode could not authenticate with OpenRouter, so the run tested nothing."
            : null;

    public Transcript ParseTranscript(string stdout) => TranscriptParsers.OpenCodeJson(stdout);

    private static string? OpenRouterKey()
    {
        try
        {
            using var auth = JsonDocument.Parse(File.ReadAllText(UserAuthFile));
            return auth.RootElement.TryGetProperty("openrouter", out var entry) && entry.TryGetProperty("key", out var key) ? key.GetString() : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

/// <summary>
/// GitHub Copilot CLI, headless (`copilot -p --yolo --output-format json`). COPILOT_HOME points into the
/// eval root's homes folder, so the developer's own Copilot hooks, MCP servers and settings take no part.
/// COPILOT_ALLOW_ALL=true trusts the working directory without a prompt, which is what loads its hooks.
/// </summary>
/// <remarks>
/// Copilot keeps its token in the operating system's credential store, keyed by account; its config.json
/// only records which account is signed in. The home's config.json is seeded with that (non-secret)
/// account record from the developer's own, so the existing sign-in is used and no token is copied.
/// </remarks>
public sealed class CopilotHarness(EvalPaths paths) : IHarness
{
    private string Home => Path.Combine(paths.Homes, "copilot");

    private static string UserConfig => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "config.json");

    public string Id => "copilot";

    public string DisplayName => "Copilot CLI";

    public string GatePath => ".github/hooks/gate/gate.cs";

    public IReadOnlyList<string> GeneratedFiles { get; } = [".github", ".agents"];

    public bool FailsClosedWhenGateCannotRun => true;

    public string? Executable { get; } = Processes.Find("copilot");

    public Task<string?> NotReadyAsync() => Task.FromResult(
        Executable is null ? "not installed"
        : SignedInAccount() is null ? "no signed-in Copilot account in ~/.copilot/config.json. Sign in once with: copilot login"
        : null);

    public Task<string> VersionAsync() => Harnesses.VersionOf(Executable!, paths.Root, "--version");

    public ProcessStartInfo CreateRun(RunRequest request)
    {
        Directory.CreateDirectory(Home);
        File.WriteAllText(Path.Combine(Home, "config.json"), SignedInAccount()!);

        List<string> args = ["-p", request.Prompt, "--yolo", "--output-format", "json", "--log-dir", Path.Combine(request.Workspace.RunDirectory, "copilot-logs")];
        if (request.Model is not null)
        {
            args.AddRange(["--model", request.Model]);
        }

        if (request.WithIntentStub)
        {
            var (command, stubArgs) = McpStub.LaunchCommand(request.Workspace.Root, request.Workspace.StubLogPath);
            var config = Path.Combine(request.Workspace.RunDirectory, "copilot-mcp.json");
            File.WriteAllText(config, JsonSerializer.Serialize(new
            {
                mcpServers = new Dictionary<string, object>
                {
                    [McpStub.ServerName] = new { type = "local", command, args = stubArgs, tools = new[] { "*" } },
                },
            }));
            args.AddRange(["--additional-mcp-config", "@" + config]);
        }

        var startInfo = Processes.Command(Executable!, request.Workspace.Root, [.. args]);
        startInfo.Environment["COPILOT_HOME"] = Home;
        startInfo.Environment["COPILOT_ALLOW_ALL"] = "true";
        return startInfo;
    }

    public string? SignInFailure(ProcessResult process) =>
        (process.Stdout + process.Stderr).Contains("not logged in", StringComparison.OrdinalIgnoreCase)
        || (process.Stdout + process.Stderr).Contains("copilot login", StringComparison.OrdinalIgnoreCase)
            ? "Copilot CLI is not signed in, so the run tested nothing. Sign in with: copilot login"
            : null;

    public Transcript ParseTranscript(string stdout) => TranscriptParsers.CopilotJson(stdout);

    /// <summary>The developer's signed-in account record, as a minimal config.json - or null when there is none.</summary>
    private static string? SignedInAccount()
    {
        try
        {
            // The file starts with // comments, which JsonDocument skips when told to.
            using var config = JsonDocument.Parse(File.ReadAllText(UserConfig), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
            var root = config.RootElement;
            return root.TryGetProperty("lastLoggedInUser", out var last) && root.TryGetProperty("loggedInUsers", out var users)
                ? JsonSerializer.Serialize(new { lastLoggedInUser = last, loggedInUsers = users })
                : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
