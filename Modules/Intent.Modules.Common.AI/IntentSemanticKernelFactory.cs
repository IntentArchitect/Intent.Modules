using System;
using System.Net.Http;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.AI.ChatCompletionServices;
using Intent.Modules.Common.AI.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OllamaSharp;

namespace Intent.Modules.Common.AI;

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
        //var settings = _userSettingsProvider.GetAISettings();
        //var model = string.IsNullOrWhiteSpace(settings.Model()) ? "gpt-4.1" : settings.Model();
        throw new NotSupportedException("Obsolete");
        //return BuildSemanticKernel(model, configure);
    }

    public Kernel BuildSemanticKernel(string model, Action<IKernelBuilder>? configure)
    {
        var settings = _userSettingsProvider.GetAISettings();
        var provider = settings.Provider().AsEnum();
        return BuildSemanticKernel(model, provider, configure);
    }

    public Kernel BuildSemanticKernel(string model, AISettings.ProviderOptionsEnum provider, Action<IKernelBuilder>? configure)
    {
        var settings = _userSettingsProvider.GetAISettings();
        string? apiKey;
        
        // Create the Semantic Kernel instance with your LLM service.
        // Replace <your-openai-key> with your actual OpenAI API key and adjust the model name as needed.
        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging(configure: b => b.AddProvider(provider: new SoftwareFactoryLoggingProvider()).SetMinimumLevel(level: LogLevel.Trace));
        builder.Services.ConfigureHttpClientDefaults(configure: defaults =>
        {
            defaults.ConfigureHttpClient(configureClient: client =>
            {
                client.Timeout = TimeSpan.FromSeconds(value: 180); // 3 min
            });
        });

        if (configure != null)
        {
            configure(obj: builder);
        }

        switch (provider)
        {
            case AISettings.ProviderOptionsEnum.OpenAi:
                apiKey = settings.OpenAIAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "OPENAI_API_KEY");
                }

                builder.Services.AddOpenAIChatCompletion(
                    modelId: model,
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("OPENAI_API_KEY")));
                break;
            
            case AISettings.ProviderOptionsEnum.AzureOpenAi:
                apiKey = settings.AzureOpenAIAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "AZURE_OPENAI_API_KEY");
                }

                builder.Services.AddAzureOpenAIChatCompletion(
                    deploymentName: settings.DeploymentName(),
                    endpoint: settings.AzureOpenAIAPIUrl(),
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("AZURE_OPENAI_API_KEY")),
                    modelId: model);
                break;
            
            case AISettings.ProviderOptionsEnum.Ollama:
                builder.Services.AddOllamaChatCompletion(
                    ollamaClient: new OllamaApiClient(
                        client: new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(value: 10), // Running this locally could be slow
                            BaseAddress = new Uri(uriString: settings.OllamaAPIUrl())
                        },
                        defaultModel: model)
                );
                break;
            
            case AISettings.ProviderOptionsEnum.Anthropic:
                apiKey = settings.AnthropicAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "ANTHROPIC_API_KEY");
                }

                builder.Services.AddAnthropicChatCompletion(
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("ANTHROPIC_API_KEY")),
                    model: model,
                    maxTokens: int.TryParse(s: _userSettingsProvider.GetAISettings().MaxTokens(), result: out var maxTokens) ? maxTokens : null);
                break;
            
            case AISettings.ProviderOptionsEnum.OpenRouter:
                apiKey = settings.OpenRouterAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "OPENROUTER_API_KEY");
                }
                
                builder.Services.AddOpenRouterChatCompletion(
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("OPENROUTER_API_KEY")),
                    modelId: model);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(provider), message: $"Unknown provider: {provider}");
        }

        var kernel = builder.Build();
        return kernel;
    }

    private static string GetErrorMessage(string environmentKeyName)
    {
        return $"No API Key defined. Update this in your User Settings -> AI Settings or set the `{environmentKeyName.Replace("_", "\\_")}` environment variable. Documentation [here](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html#user-settings)";
    }
}