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
        var azureDeploymentName = _userSettingsProvider.GetAISettings().AzureOpenAIDeploymentName();
        var openAiCompatibleModelName = _userSettingsProvider.GetAISettings().OpenAICompatibleModel();
        var ollamaModelName = _userSettingsProvider.GetAISettings().OllamaModel();
        
        List<ModelRecord> providerModels =
        [
            new("open-ai",              "gpt-4.1",                              "OpenAI",               ThinkingType.None),
            new("open-ai",              "gpt-4o",                               "OpenAI",               ThinkingType.None),
            new("open-ai",              "gpt-5",                                "OpenAI",               ThinkingType.ThinkingLevels),
            new("open-ai",              "gpt-5.1",                              "OpenAI",               ThinkingType.ThinkingLevels),
            new("open-ai",              "o1",                                   "OpenAI",               ThinkingType.ThinkingLevels),
            new("open-ai",              "o3",                                   "OpenAI",               ThinkingType.ThinkingLevels),
            new("anthropic",            "claude-haiku-4-5-20251001",            "Anthropic",            ThinkingType.ThinkingLevels),
            new("anthropic",            "claude-sonnet-4-5-20250929",           "Anthropic",            ThinkingType.ThinkingLevels),
            new("anthropic",            "claude-opus-4-1-20250805",             "Anthropic",            ThinkingType.ThinkingLevels),
            new("anthropic",            "claude-opus-4-20250514",               "Anthropic",            ThinkingType.ThinkingLevels),
            new("anthropic",            "claude-sonnet-4-20250514",             "Anthropic",            ThinkingType.ThinkingLevels),
            new("anthropic",            "claude-3-7-sonnet-20250219",           "Anthropic",            ThinkingType.ThinkingLevels),
            new("open-router",          "x-ai/grok-4",                          "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "z-ai/glm-4.6",                         "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "google/gemini-2.5-pro",                "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "deepseek/deepseek-v3.2-exp",           "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "qwen/qwen3-coder",                     "OpenRouter",           ThinkingType.None),
            new("open-router",          "qwen/qwen3-coder-plus",                "OpenRouter",           ThinkingType.None),
            new("open-router",          "qwen/qwen3-max",                       "OpenRouter",           ThinkingType.None),
            new("open-router",          "qwen/qwen3-235b-a22b-thinking-2507",   "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "anthropic/claude-haiku-4.5",           "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "anthropic/claude-sonnet-4.5",          "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "anthropic/claude-opus-4.1",            "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "anthropic/claude-opus-4",              "OpenRouter",           ThinkingType.ThinkingLevels),
            new("open-router",          "anthropic/claude-sonnet-4",            "OpenRouter",           ThinkingType.ThinkingLevels),
            new("google-gemini",        "gemini-2.5-pro",                       "Google Gemini",        ThinkingType.ThinkingLevels),
            new("google-gemini",        "gemini-2.5-flash",                     "Google Gemini",        ThinkingType.None),
            new("azure-open-ai",        azureDeploymentName,                    "Azure OpenAI",         ThinkingType.Unknown),
            new("open-ai-compatible",   openAiCompatibleModelName,              "Open AI Compatible",   ThinkingType.Unknown),
            new("ollama",               ollamaModelName,                        "Ollama",               ThinkingType.Unknown)
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
        var openAiCompatibleModel = (
                                        !string.IsNullOrWhiteSpace(_userSettingsProvider.GetAISettings().OpenAICompatibleAPIKey()) ||
                                        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPENAI_COMPATIBLE_API_KEY"))
                                    ) &&
                                    !string.IsNullOrWhiteSpace(openAiCompatibleModelName);
        var hasOllamaModel = !string.IsNullOrWhiteSpace(ollamaModelName);
        
        // Let's filter these models based on whether API keys (or their environment variable counterparts) are set.
        providerModels = providerModels.FindAll(model =>
            model.ProviderId switch
            {
                "open-ai" => hasOpenAiKey,
                "azure-open-ai" => hasAzureOpenAiKey,
                "anthropic" => hasAnthropicKey,
                "open-router" => hasOpenRouterKey,
                "google-gemini" => hasGoogleGeminiKey,
                "open-ai-compatible" => openAiCompatibleModel,
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

    internal enum ThinkingType
    {
        /// <summary>
        /// Model does not have a thinking/reasoning mode.
        /// </summary>
        None,
        /// <summary>
        /// Unknown if the model has a thinking/reasoning mode.
        /// </summary>
        Unknown,
        /// <summary>
        /// Model has reasoning capabilities that can be set to high and low.
        /// </summary>
        ThinkingLevels
    }
    
    private record ModelRecord(string ProviderId, string ModelName, string ProviderName, ThinkingType ThinkingType);
}