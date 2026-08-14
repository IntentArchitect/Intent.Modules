using System;
using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Intent.Modules.Common.AI;

public interface IAiProviderService
{
    PromptExecutionSettings GetPromptExecutionSettings(string thinkingLevel);
}

internal class AiProviderService : IAiProviderService
{
    private delegate PromptExecutionSettings PromptExecutionSettingsFactory(string thinkingLevel);

    private readonly PromptExecutionSettingsFactory _promptExecutionSettingsFactory;

    private AiProviderService(PromptExecutionSettingsFactory promptExecutionSettingsFactory)
    {
        ArgumentNullException.ThrowIfNull(nameof(promptExecutionSettingsFactory));

        _promptExecutionSettingsFactory = promptExecutionSettingsFactory;
    }

    public PromptExecutionSettings GetPromptExecutionSettings(string thinkingLevel)
    {
        return _promptExecutionSettingsFactory(thinkingLevel);
    }

    public static IAiProviderService CreateOpenAiService()
    {
        return new AiProviderService(thinkingLevel => new OpenAIPromptExecutionSettings
        {
            ReasoningEffort = thinkingLevel == "none" ? null : thinkingLevel,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        });
    }

    public static IAiProviderService CreateAzureOpenAiService()
    {
        return new AiProviderService(thinkingLevel => new AzureOpenAIPromptExecutionSettings
        {
            ReasoningEffort = thinkingLevel == "none" ? null : thinkingLevel,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        });
    }

    public static IAiProviderService CreateAnthropicService()
    {
        return new AiProviderService(thinkingLevel =>
        {
            Dictionary<string, object>? extensionData = null;
            if (!"none".Equals(thinkingLevel))
            {
                extensionData = new Dictionary<string, object>
                {
                    ["thinking"] = new
                    {
                        type = thinkingLevel == "low" ? "disabled" : thinkingLevel == "high" ? "enabled" : null
                    }
                };
            }

            return new PromptExecutionSettings
            {
                ExtensionData = extensionData,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
        });
    }

    public static IAiProviderService CreateOpenRouterService()
    {
        return new AiProviderService(thinkingLevel =>
        {
            Dictionary<string, object>? extensionData = null;
            if (!"none".Equals(thinkingLevel))
            {
                extensionData = new Dictionary<string, object>
                {
                    // OpenAI equivalent
                    ["reasoning"] = new
                    {
                        effort = thinkingLevel
                    },
                    // Anthropic equivalent
                    ["thinking"] = new
                    {
                        type = thinkingLevel == "low" ? "disabled" : thinkingLevel == "high" ? "enabled" : null
                    }
                };
            }

            return new PromptExecutionSettings
            {
                ExtensionData = extensionData,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
        });
    }

    public static IAiProviderService CreateGoogleGeminiService()
    {
        return new AiProviderService(thinkingLevel =>
        {
            var config = new GeminiThinkingConfig();
            if (!"none".Equals(thinkingLevel))
            {
                config.ThinkingBudget = thinkingLevel == "low" ? null : thinkingLevel == "high" ? 1024 : null;
            }

            return new GeminiPromptExecutionSettings
            {
                ThinkingConfig = config,
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
            };
        });
    }

    public static IAiProviderService CreateOllamaService()
    {
        return new AiProviderService(thinkingLevel =>
        {
            Dictionary<string, object>? extensionData = null;
            if (!"none".Equals(thinkingLevel))
            {
                extensionData = new Dictionary<string, object>
                {
                    // OpenAI equivalent
                    ["reasoning"] = new
                    {
                        effort = thinkingLevel
                    },
                    // Anthropic equivalent
                    ["thinking"] = new
                    {
                        type = thinkingLevel == "low" ? "disabled" : thinkingLevel == "high" ? "enabled" : null
                    }
                };
            }

            return new OllamaPromptExecutionSettings
            {
                ExtensionData = extensionData,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
        });
    }
}