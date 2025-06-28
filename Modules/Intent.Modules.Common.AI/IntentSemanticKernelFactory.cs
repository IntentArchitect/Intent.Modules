using System;
using System.Net.Http;
using Anthropic.SDK;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.AI.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;

namespace Intent.Modules.Common.AI;

#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001

#nullable enable

public class IntentSemanticKernelFactory
{
    private readonly IUserSettingsProvider _userSettingsProvider;

    public IntentSemanticKernelFactory(IUserSettingsProvider userSettingsProvider)
    {
        _userSettingsProvider = userSettingsProvider;
    }

    public Kernel BuildSemanticKernel() => BuildSemanticKernel(null);
    
    public Kernel BuildSemanticKernel(Action<IKernelBuilder>? configure)
    {
        var settings = _userSettingsProvider.GetAISettings();
        var model = string.IsNullOrWhiteSpace(settings.Model()) ? "gpt-4o" : settings.Model();
        string? apiKey;
        
        // Create the Semantic Kernel instance with your LLM service.
        // Replace <your-openai-key> with your actual OpenAI API key and adjust the model name as needed.
        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging(b => b.AddProvider(new SoftwareFactoryLoggingProvider()).SetMinimumLevel(LogLevel.Trace));
        if (configure != null)
        {
            configure(builder);
        }

        var provider = settings.Provider().AsEnum();
        switch (provider)
        {
            case AISettings.ProviderOptionsEnum.OpenAi:
                apiKey = settings.OpenAIAPIKey();
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                }

                builder.Services.AddOpenAIChatCompletion(
                    modelId: model,
                    apiKey: apiKey ?? throw new FriendlyException(GetErrorMessage("OPENAI_API_KEY")));
                break;
            
            case AISettings.ProviderOptionsEnum.AzureOpenAi:
                apiKey = settings.AzureOpenAIAPIKey();
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
                }

                builder.Services.AddAzureOpenAIChatCompletion(
                    deploymentName: settings.DeploymentName(),
                    endpoint: settings.APIUrl(),
                    apiKey: apiKey ?? throw new FriendlyException(GetErrorMessage("AZURE_OPENAI_API_KEY")),
                    modelId: model);
                break;
            
            case AISettings.ProviderOptionsEnum.Ollama:
                builder.Services.AddOllamaChatCompletion(
                    new OllamaApiClient(
                        new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(10), // Running this locally could be slow
                            BaseAddress = new Uri(settings.APIUrl())
                        },
                        model)
                );
                break;
            
            case AISettings.ProviderOptionsEnum.Anthropic:
                apiKey = settings.AnthropicAPIKey();
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
                }

                var apiAuth = new APIAuthentication(apiKey ?? throw new FriendlyException(GetErrorMessage("ANTHROPIC_API_KEY")));
                builder.Services.AddTransient((sp) =>
                {
                    var skChatService =
                        new ChatClientBuilder(new AnthropicClient(apiAuth).Messages)
                            .ConfigureOptions(x =>
                            {
                                x.ModelId = model;
                                x.MaxOutputTokens = int.TryParse(_userSettingsProvider.GetAISettings().MaxTokens(), out var maxTokens) ? maxTokens : null;
                            })
                            .UseFunctionInvocation()
                            .Build(sp)
                            .AsChatCompletionService(sp);
                    return skChatService;
                });
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), $"Unknown provider: {provider}");
        }

        var kernel = builder.Build();
        return kernel;
    }

    private string GetErrorMessage(string environmentKeyName)
    {
        return $"No API Key defined. Update this in your User Settings -> AI Settings or set the `{environmentKeyName.Replace("_", "\\_")}` environment variable. Documentation [here](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html#user-settings)";
    }
}