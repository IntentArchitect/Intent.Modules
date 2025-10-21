using System;
using System.Net.Http;
using Anthropic.SDK;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0001

namespace Intent.Modules.Common.AI.ChatCompletionServices;

internal static class AnthropicChatCompletionService
{
    /// <remarks>
    /// There is no Semantic Kernel connector for Anthropic (and neither will there be)
    /// but there is a library that implements the IChatClient interface which can be
    /// converted into a Semantic Kernel ChatCompletionService.
    /// </remarks>
    public static IServiceCollection AddAnthropicChatCompletion(
        this IServiceCollection services,
        string apiKey,
        string model,
        int? maxTokens)
    {
        var apiAuth = new APIAuthentication(apiKey);
        services.AddTransient(sp =>
        {
            var skChatService = new ChatClientBuilder(new AnthropicClient(apiAuth, new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5)
                }).Messages)
                .ConfigureOptions(x =>
                {
                    x.ModelId = model;
                    x.MaxOutputTokens = maxTokens;
                })
                .UseFunctionInvocation()
                .Build(sp)
                .AsChatCompletionService(sp);
            return skChatService;
        });
        return services;
    }
}