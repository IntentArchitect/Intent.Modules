using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Engine;
using Intent.Modules.Common.AI.Settings;
using Intent.Plugins;

namespace Intent.Modules.Common.AI.Tasks;

public class ProviderModelsTask : IModuleTask
{
    private readonly IUserSettingsProvider _userSettingsProvider;

    public ProviderModelsTask(IUserSettingsProvider userSettingsProvider)
    {
        _userSettingsProvider = userSettingsProvider;
    }
    
    public string TaskTypeId => "Intent.Modules.Common.AI.Tasks.ProviderModelsTask";
    public string TaskTypeName => "Fetch Provider Models";
    public int Order => 0;
    
    public string Execute(params string[] args)
    {
        var azureDeploymentName = _userSettingsProvider.GetAISettings().DeploymentName() ?? "Unspecified";
        var ollamaModelName = _userSettingsProvider.GetAISettings().OllamaModel() ?? "Unspecified";
        
        List<ModelRecord> providerModels =
        [
            new("azure-open-ai",    azureDeploymentName,                              "Azure OpenAI", ThinkingType.ToggleThinking),
            new("open-ai",          "gpt-4.1",                              "OpenAI",       ThinkingType.None),
            new("open-ai",          "gpt-4o",                               "OpenAI",       ThinkingType.None),
            new("open-ai",          "gpt-5",                                "OpenAI",       ThinkingType.ThinkingLevels),
            new("open-ai",          "gpt-5-codex",                          "OpenAI",       ThinkingType.ThinkingLevels),
            new("open-ai",          "o1",                                   "OpenAI",       ThinkingType.ThinkingLevels),
            new("open-ai",          "o1-pro",                               "OpenAI",       ThinkingType.ThinkingLevels),
            new("open-ai",          "o3",                                   "OpenAI",       ThinkingType.ThinkingLevels),
            new("anthropic",        "claude-sonnet-4-5-20250929",           "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-opus-4-1-20250805",             "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-opus-4-20250514",               "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-sonnet-4-20250514",             "Anthropic",    ThinkingType.ToggleThinking),
            new("open-router",      "openrouter/auto",                      "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "x-ai/grok-4",                          "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "z-ai/glm-4.6",                         "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "google/gemini-2.5-pro",                "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "deepseek/deepseek-r1-0528",            "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "deepseek/deepseek-v3.2-exp",           "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "qwen/qwen3-coder",                     "OpenRouter",   ThinkingType.None),
            new("open-router",      "qwen/qwen3-coder-plus",                "OpenRouter",   ThinkingType.None),
            new("open-router",      "qwen/qwen3-max",                       "OpenRouter",   ThinkingType.None),
            new("open-router",      "qwen/qwen3-235b-a22b-thinking-2507",   "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "anthropic/claude-sonnet-4",            "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-opus-4",              "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-opus-4.1",            "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-sonnet-4.5",          "OpenRouter",   ThinkingType.ToggleThinking),
            new("ollama",           ollamaModelName,                                  "Ollama",       ThinkingType.None)
        ];
        
        // Let's filter these models based on whether API keys (or their environment variable counterparts) are set.
        var settings = _userSettingsProvider.GetAISettings();
        providerModels = providerModels.FindAll(model =>
            model.ProviderId switch
            {
                "open-ai" => !string.IsNullOrWhiteSpace(settings.OpenAIAPIKey()) ||
                             !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPENAI_API_KEY")),
                "azure-open-ai" => !string.IsNullOrWhiteSpace(settings.AzureOpenAIAPIKey()) ||
                             !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")),
                "anthropic" => !string.IsNullOrWhiteSpace(settings.AnthropicAPIKey()) ||
                               !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")),
                "open-router" => !string.IsNullOrWhiteSpace(settings.OpenRouterAPIKey()) ||
                                 !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")),
                "ollama" => !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().OllamaModel()),
                _ => false
            });
        
        return JsonSerializer.Serialize(providerModels, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
    }

    private enum ThinkingType
    {
        /// <summary>
        /// Model is not nor does it have a reason/thinking mode.
        /// </summary>
        None,
        /// <summary>
        /// Thinking/reasoning can be enabled but is not on by default.
        /// </summary>
        ToggleThinking,
        /// <summary>
        /// This is a thinking/reasoning model, but it can operate at various levels.
        /// </summary>
        ThinkingLevels
    }
    
    private record ModelRecord(string ProviderId, string ModelName, string ProviderName, ThinkingType ThinkingType);
}