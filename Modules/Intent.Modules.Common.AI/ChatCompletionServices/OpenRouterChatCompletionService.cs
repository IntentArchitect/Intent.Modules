using System;
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
            BaseAddress = new Uri("https://openrouter.ai/api/v1/")
        });
        return services;
    }

    public class OpenRouterErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Attempt to parse OpenRouter error format
                var normalizedError = NormalizeOpenRouterError(content, response.StatusCode);
                throw new OpenAICompatibleException(normalizedError);
            }

            return response;
        }

        private OpenAIError NormalizeOpenRouterError(string content, System.Net.HttpStatusCode statusCode)
        {
            // Example: Map OpenRouter error fields to OpenAI error fields
            try
            {
                var openRouterError = JsonSerializer.Deserialize<OpenRouterError>(content);
                return new OpenAIError
                {
                    Code = (int)statusCode,
                    Type = openRouterError?.Type ?? "unknown_error",
                    Message = openRouterError?.Message ?? content
                };
            }
            catch
            {
                // Fallback for unexpected error formats
                return new OpenAIError
                {
                    Code = (int)statusCode,
                    Type = "unknown_error",
                    Message = content
                };
            }
        }
    }

// Example error classes
    public class OpenRouterError
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class OpenAIError
    {
        public int Code { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class OpenAICompatibleException : Exception
    {
        public OpenAIError Error { get; }

        public OpenAICompatibleException(OpenAIError error) : base(error.Message)
        {
            Error = error;
        }
    }
}