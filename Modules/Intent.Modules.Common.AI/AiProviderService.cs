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
    PromptExecutionSettings GetPromptExecutionSettings(string thinkingType);
}

internal class AiProviderService : IAiProviderService
{
    private delegate PromptExecutionSettings PromptExecutionSettingsFactory(string thinkingType);
    
    private readonly PromptExecutionSettingsFactory _promptExecutionSettingsFactory;
    
    private AiProviderService(PromptExecutionSettingsFactory promptExecutionSettingsFactory)
    {
        ArgumentNullException.ThrowIfNull(nameof(promptExecutionSettingsFactory));
        
        _promptExecutionSettingsFactory = promptExecutionSettingsFactory;
    }
    
    public PromptExecutionSettings GetPromptExecutionSettings(string thinkingType)
    {
        return _promptExecutionSettingsFactory(thinkingType);
    }
    
    public static IAiProviderService CreateOpenAiService()
    {
        return new AiProviderService(thinkingType => new OpenAIPromptExecutionSettings
        {
            ReasoningEffort = thinkingType,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        });
    }

    public static IAiProviderService CreateAzureOpenAiService()
    {
        return new AiProviderService(thinkingType => new AzureOpenAIPromptExecutionSettings
        {
            ReasoningEffort = thinkingType,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        });
    }

    public static IAiProviderService CreateAnthropicService()
    {
        return new AiProviderService(thinkingType => new PromptExecutionSettings
        {
            ExtensionData = new Dictionary<string, object>
            {
                ["thinking"] = new
                {
                    type = thinkingType == "low" ? "disabled" : thinkingType == "high" ? "enabled" : null
                }
            },
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        });
    }

    public static IAiProviderService CreateOpenRouterService()
    {
        return new AiProviderService(thinkingType => new PromptExecutionSettings
        {
            ExtensionData = new Dictionary<string, object>
            {
                // OpenAI equivalent
                ["reasoning"] = new
                {
                    effort = thinkingType
                },
                // Anthropic equivalent
                ["thinking"] = new
                {
                    type = thinkingType == "low" ? "disabled" : thinkingType == "high" ? "enabled" : null
                }
            },
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        });
    }

    public static IAiProviderService CreateGoogleGeminiService()
    {
        return new AiProviderService(thinkingType => new GeminiPromptExecutionSettings
        {
            ThinkingConfig = new GeminiThinkingConfig()
            {
                ThinkingBudget = thinkingType == "low" ? null : thinkingType == "high" ? 1024 : null
            },
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
        });
    }

    public static IAiProviderService CreateOllamaService()
    {
        return new AiProviderService(thinkingType => new OllamaPromptExecutionSettings
        {
            ExtensionData = new Dictionary<string, object>
            {
                // OpenAI equivalent
                ["reasoning"] = new
                {
                    effort = thinkingType
                },
                // Anthropic equivalent
                ["thinking"] = new
                {
                    type = thinkingType == "low" ? "disabled" : thinkingType == "high" ? "enabled" : null
                }
            },
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        });
    }
}