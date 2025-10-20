using System;
using System.Collections.Generic;
using System.Linq;
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
            new("azure-open-ai",    azureDeploymentName,                    "Azure OpenAI", ThinkingType.ToggleThinking),
            new("open-ai",          "gpt-4.1",                              "OpenAI",       ThinkingType.None),
            new("open-ai",          "gpt-4o",                               "OpenAI",       ThinkingType.None),
            new("open-ai",          "gpt-5",                                "OpenAI",       ThinkingType.ThinkingLevels),
            new("open-ai",          "o1",                                   "OpenAI",       ThinkingType.ThinkingLevels),
            new("open-ai",          "o3",                                   "OpenAI",       ThinkingType.ThinkingLevels),
            new("anthropic",        "claude-haiku-4-5-20251001",            "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-sonnet-4-5-20250929",           "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-opus-4-1-20250805",             "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-opus-4-20250514",               "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-sonnet-4-20250514",             "Anthropic",    ThinkingType.ToggleThinking),
            new("anthropic",        "claude-3-7-sonnet-20250219",           "Anthropic",    ThinkingType.ToggleThinking),
            new("open-router",      "openrouter/auto",                      "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "x-ai/grok-4",                          "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "z-ai/glm-4.6",                         "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "google/gemini-2.5-pro",                "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "deepseek/deepseek-v3.2-exp",           "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "qwen/qwen3-coder",                     "OpenRouter",   ThinkingType.None),
            new("open-router",      "qwen/qwen3-coder-plus",                "OpenRouter",   ThinkingType.None),
            new("open-router",      "qwen/qwen3-max",                       "OpenRouter",   ThinkingType.None),
            new("open-router",      "qwen/qwen3-235b-a22b-thinking-2507",   "OpenRouter",   ThinkingType.ThinkingLevels),
            new("open-router",      "anthropic/claude-haiku-4.5",           "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-sonnet-4.5",          "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-opus-4.1",            "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-opus-4",              "OpenRouter",   ThinkingType.ToggleThinking),
            new("open-router",      "anthropic/claude-sonnet-4",            "OpenRouter",   ThinkingType.ToggleThinking),
            new("google-gemini",    "gemini-2.5-pro",                       "Google Gemini",ThinkingType.ToggleThinking),
            new("google-gemini",    "gemini-2.5-flash",                     "Google Gemini",ThinkingType.None),
            new("ollama",           ollamaModelName,                        "Ollama",       ThinkingType.None)
        ];
        
        var hasOpenAiKey = !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().OpenAIAPIKey()) ||
                          !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        var hasAzureOpenAiKey = !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().AzureOpenAIAPIKey()) ||
                               !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY"));
        var hasAnthropicKey = !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().AnthropicAPIKey()) ||
                             !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"));
        var hasOpenRouterKey = !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().OpenRouterAPIKey()) ||
                              !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPENROUTER_API_KEY"));
        var hasGoogleGeminiKey = !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().GoogleGeminiAPIKey()) ||
                                 !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOOGLE_API_KEY")) ||
                                 !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GEMINI_API_KEY"));
        var hasOllamaModel = !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().OllamaModel());
        
        // Let's filter these models based on whether API keys (or their environment variable counterparts) are set.
        providerModels = providerModels.FindAll(model =>
            model.ProviderId switch
            {
                "open-ai" => hasOpenAiKey,
                "azure-open-ai" => hasAzureOpenAiKey,
                "anthropic" => hasAnthropicKey,
                "open-router" => hasOpenRouterKey,
                "google-gemini" => hasGoogleGeminiKey,
                "ollama" => hasOllamaModel,
                _ => false
            });

        providerModels = providerModels.OrderBy(x => x.ProviderName).ThenBy(x => x.ModelName).ToList();
        
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