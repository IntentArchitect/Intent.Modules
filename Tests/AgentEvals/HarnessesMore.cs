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

    /// <summary>
    /// Z.ai's GLM 5.2 through OpenRouter: tool calling, and not an Anthropic model - Claude is only ever
    /// run through Claude Code's own sign-in. All three segments are needed: provider, then OpenRouter's
    /// own model id, which itself contains a slash.
    /// </summary>
    public const string DefaultModel = "openrouter/z-ai/glm-5.2";

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
            "-m", request.Model ?? DefaultModel, request.Prompt);
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

    /// <summary>
    /// A small OpenAI model, never Copilot's own default (which may be an Anthropic one). Copilot bills by
    /// usage on every plan since June 2026, so no model is free; this one keeps the cost lowest.
    /// </summary>
    public const string DefaultModel = "gpt-5-mini";

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

        List<string> args =
        [
            "-p", request.Prompt, "--yolo", "--output-format", "json", "--log-dir", Path.Combine(request.Workspace.RunDirectory, "copilot-logs"),
            "--model", request.Model ?? DefaultModel,
        ];

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

/// <summary>
/// Kiro CLI, headless (`kiro-cli chat --v3 --no-interactive --trust-all-tools --output-format stream-json`).
/// Workspace hooks load in v3 non-interactive runs from kiro-cli 2.27.1; earlier versions do not run them.
/// </summary>
/// <remarks>
/// Kiro has no home override, so the developer's own Kiro config takes part - including any MCP servers
/// in ~/.kiro/settings/mcp.json, such as the real Intent Architect server. Each workspace therefore gets
/// a .kiro/settings/mcp.json defining "intent-architect" as the runner's stub, or as nothing at all:
/// a workspace server overrides a user one of the same name (checked with "kiro-cli mcp list"). That
/// file is listed in the workspace's .git/info/exclude, so it is not part of what the agent changed.
/// Kiro signs in with the developer's own Kiro account; its sign-in is not copied anywhere.
/// </remarks>
public sealed class KiroHarness(EvalPaths paths) : IHarness
{
    public string Id => "kiro";

    public string DisplayName => "Kiro CLI";

    public string GatePath => ".kiro/hooks/gate/gate.cs";

    public IReadOnlyList<string> GeneratedFiles { get; } = [".kiro"];

    /// <summary>Kiro lets an action through when a hook cannot run at all - it has no fail-closed setting.</summary>
    public bool FailsClosedWhenGateCannotRun => false;

    public string? Executable { get; } = Processes.Find("kiro-cli");

    public async Task<string?> NotReadyAsync()
    {
        if (Executable is null)
        {
            return "not installed";
        }

        var whoami = await Processes.RunAsync(Processes.Command(Executable, paths.Root, "whoami"), null, TimeSpan.FromSeconds(30));
        return whoami.ExitCode == 0 ? null : "not signed in. Sign in once with: kiro-cli login";
    }

    public Task<string> VersionAsync() => Harnesses.VersionOf(Executable!, paths.Root, "--version");

    public ProcessStartInfo CreateRun(RunRequest request)
    {
        var settings = Path.Combine(request.Workspace.Root, ".kiro", "settings");
        Directory.CreateDirectory(settings);
        object server = new { command = "cmd", args = new[] { "/c", "exit", "1" }, disabled = true };
        if (request.WithIntentStub)
        {
            var (command, stubArgs) = McpStub.LaunchCommand(request.Workspace.Root, request.Workspace.StubLogPath);
            server = new { command, args = stubArgs };
        }

        File.WriteAllText(Path.Combine(settings, "mcp.json"), JsonSerializer.Serialize(new
        {
            mcpServers = new Dictionary<string, object> { [McpStub.ServerName] = server },
        }));
        File.AppendAllText(Path.Combine(request.Workspace.Root, ".git", "info", "exclude"), "\n.kiro/settings/\n");

        List<string> args = ["chat", "--v3", "--no-interactive", "--trust-all-tools", "--output-format", "stream-json"];
        if (request.Model is not null)
        {
            args.AddRange(["--model", request.Model]);
        }

        args.Add(request.Prompt);
        return Processes.Command(Executable!, request.Workspace.Root, [.. args]);
    }

    public string? SignInFailure(ProcessResult process) =>
        (process.Stdout + process.Stderr).Contains("kiro-cli login", StringComparison.OrdinalIgnoreCase)
            ? "Kiro CLI is not signed in, so the run tested nothing. Sign in with: kiro-cli login"
            : null;

    public Transcript ParseTranscript(string stdout) => TranscriptParsers.KiroStreamJson(stdout);
}
