using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.AI.ChatCompletionServices;
using Intent.Modules.Common.AI.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OllamaSharp;

namespace Intent.Modules.Common.AI;

public class IntentSemanticKernelFactory
{
    private readonly IUserSettingsProvider _userSettingsProvider;

    public IntentSemanticKernelFactory(IUserSettingsProvider userSettingsProvider)
    {
        _userSettingsProvider = userSettingsProvider;
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
        
        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging(configure: b => b.AddProvider(provider: new SoftwareFactoryLoggingProvider()).SetMinimumLevel(level: LogLevel.Trace));
        builder.Services.ConfigureHttpClientDefaults(configure: defaults =>
        {
            defaults.ConfigureHttpClient(configureClient: client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
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

                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateOpenAiService());
                
                break;
            
            case AISettings.ProviderOptionsEnum.AzureOpenAi:
                apiKey = settings.AzureOpenAIAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "AZURE_OPENAI_API_KEY");
                }

                builder.Services.AddAzureOpenAIChatCompletion(
                    deploymentName: settings.AzureOpenAIDeploymentName(),
                    endpoint: settings.AzureOpenAIAPIUrl(),
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("AZURE_OPENAI_API_KEY")),
                    modelId: model);
                
                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateAzureOpenAiService());
                
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
                    maxTokens: int.TryParse(s: _userSettingsProvider.GetAISettings().AnthropicMaxTokens(), result: out var maxTokens) ? maxTokens : null);
                
                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateAnthropicService());
                
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
                
                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateOpenRouterService());
                
                break;
            
            case AISettings.ProviderOptionsEnum.GoogleGemini:
                apiKey = settings.GoogleGeminiAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    // Google's docs actually have both: https://ai.google.dev/gemini-api/docs/api-key#set-api-env-var
                    apiKey = Environment.GetEnvironmentVariable(variable: "GOOGLE_API_KEY")
                        ?? Environment.GetEnvironmentVariable(variable: "GEMINI_API_KEY");
                }
                
                builder.Services.AddGoogleAIGeminiChatCompletion(
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("GOOGLE_API_KEY OR GEMINI_API_KEY")),
                    modelId: model);

                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateGoogleGeminiService());
                
                break;
            
            case AISettings.ProviderOptionsEnum.OpenAiCompatible:
                apiKey = settings.OpenAICompatibleAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "OPENAI_COMPATIBLE_API_KEY");
                }

                builder.Services.AddOpenAIChatCompletion(
                    modelId: model,
                    endpoint: new Uri(settings.OpenAICompatibleAPIUrl()),
                    apiKey: !string.IsNullOrWhiteSpace(value: apiKey) 
                        ? apiKey : throw new FriendlyException(message: GetErrorMessage("OPENAI_COMPATIBLE_API_KEY")));

                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateOpenAiService());
                
                break;
            
            case AISettings.ProviderOptionsEnum.Ollama:
                apiKey = settings.OllamaAPIKey();
                if (string.IsNullOrWhiteSpace(value: apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable(variable: "OLLAMA_API_KEY");
                }
                
                var ollamaHttpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(value: 10), // Running this locally could be slow
                    BaseAddress = new Uri(uriString: settings.OllamaAPIUrl())
                };

                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    ollamaHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                }

                builder.Services.AddOllamaChatCompletion(
                    ollamaClient: new OllamaApiClient(
                        client: ollamaHttpClient,
                        defaultModel: model)
                );
                
                builder.Services.AddSingleton<IAiProviderService>(AiProviderService.CreateOllamaService());
                
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