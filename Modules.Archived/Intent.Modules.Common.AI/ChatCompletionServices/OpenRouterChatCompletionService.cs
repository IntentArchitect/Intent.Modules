using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Intent.Modules.Common.AI.ChatCompletionServices;

internal static class OpenRouterChatCompletionService
{
    /// <remarks>
    /// There is no Semantic Kernel connector for Open Router since its "compatible"
    /// with OpenAI API standards except where it isn't.
    /// We need to introduce an adapter to make it work with Semantic Kernel.
    /// </remarks>
    public static IServiceCollection AddOpenRouterChatCompletion(
        this IServiceCollection services,
        string apiKey,
        string modelId)
    {
        var httpClientHandler = new OpenRouterErrorHandler
        {
            InnerHandler = new HttpClientHandler()
        };
        services.AddOpenAIChatClient(modelId, apiKey, httpClient: new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri("https://openrouter.ai/api/v1/"),
            Timeout = TimeSpan.FromMinutes(5)
        });
        return services;
    }

    public class OpenRouterErrorHandler : DelegatingHandler
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            // Attempt to parse OpenRouter error format
            var normalizedError = GetNormalizeOpenRouterError(content, response.StatusCode);
            if (normalizedError is not null)
            {
                throw new Exception(normalizedError.Message);
            }

            return response;
        }

        private static OpenAIError? GetNormalizeOpenRouterError(string content, System.Net.HttpStatusCode statusCode)
        {
            // Example: Map OpenRouter error fields to OpenAI error fields
            try
            {
                var openRouterResponse = JsonSerializer.Deserialize<OpenRouterResponse>(content, JsonSerializerOptions);
                if (openRouterResponse?.Error is null)
                {
                    return null;
                }
                return new OpenAIError
                {
                    Code = openRouterResponse.Error.Code,
                    Message = $"{openRouterResponse.Error.Message} => {openRouterResponse.Error.Metadata?["raw"]}"
                };
            }
            catch (Exception ex)
            {
                // Fallback for unexpected error formats
                return new OpenAIError
                {
                    Code = (int)statusCode,
                    Message = ex.Message,
                };
            }
        }
    }

    public class OpenRouterResponse
    {
        public OpenRouterResponseError? Error { get; set; }
    }

    public class OpenRouterResponseError
    {
        public int Code { get; set; }
        public string Message { get; set; } = null!;
        public Dictionary<string, object>? Metadata { get; set; }
    }
    
    public class OpenAIError
    {
        public int Code { get; set; }
        public string Message { get; set; } = null!;
    }
}
