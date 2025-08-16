using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Plugins;

namespace Intent.Modules.Common.AI.Tasks;

public class ProviderModelsTask : IModuleTask
{
    public string TaskTypeId => "Intent.Modules.Common.AI.Tasks.ProviderModelsTask";
    public string TaskTypeName => "Fetch Provider Models";
    public int Order => 0;
    
    public string Execute(params string[] args)
    {
        List<ModelRecord> providerModels =
        [
            new("open-ai",     "gpt-4.1",                            "OpenAI",      ThinkingType.None),
            new("open-ai",     "gpt-4o",                             "OpenAI",      ThinkingType.None),
            new("open-ai",     "gpt-5",                              "OpenAI",      ThinkingType.ThinkingLevels),
            new("open-ai",     "o1",                                 "OpenAI",      ThinkingType.ThinkingLevels),
            new("open-ai",     "o1-pro",                             "OpenAI",      ThinkingType.ThinkingLevels),
            new("open-ai",     "o3",                                 "OpenAI",      ThinkingType.ThinkingLevels),
            new("anthropic",   "claude-opus-4-1-20250805",           "Anthropic",   ThinkingType.ToggleThinking),
            new("anthropic",   "claude-opus-4-20250514",             "Anthropic",   ThinkingType.ToggleThinking),
            new("anthropic",   "claude-sonnet-4-20250514",           "Anthropic",   ThinkingType.ToggleThinking),
            new("open-router", "x-ai/grok-4",                        "OpenRouter",  ThinkingType.ThinkingLevels),
            new("open-router", "google/gemini-2.5-pro",              "OpenRouter",  ThinkingType.ThinkingLevels),
            new("open-router", "deepseek/deepseek-r1-0528",          "OpenRouter",  ThinkingType.ThinkingLevels),
            new("open-router", "qwen/qwen3-coder",                   "OpenRouter",  ThinkingType.None),
            new("open-router", "qwen/qwen3-235b-a22b-thinking-2507", "OpenRouter",  ThinkingType.ThinkingLevels),
            new("open-router", "anthropic/claude-sonnet-4",          "OpenRouter",  ThinkingType.ToggleThinking),
            new("open-router", "anthropic/claude-opus-4",            "OpenRouter",  ThinkingType.ToggleThinking),
            new("open-router", "anthropic/claude-opus-4.1",          "OpenRouter",  ThinkingType.ToggleThinking)
        ];
        
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