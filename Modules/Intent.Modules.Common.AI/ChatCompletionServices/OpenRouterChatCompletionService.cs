using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using OpenAI.Chat;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using FunctionCallContent = Microsoft.Extensions.AI.FunctionCallContent;
using TextContent = Microsoft.Extensions.AI.TextContent;

#pragma warning disable SKEXP0001

namespace Intent.Modules.Common.AI.ChatCompletionServices;

internal static class OpenRouterChatCompletionService
{
    public static IServiceCollection AddOpenRouterChatCompletion(
        this IServiceCollection services,
        string apiKey,
        string modelId)
    {
        services.AddTransient(sp =>
        {
            var skChatService = new ChatClientBuilder(new OpenRouterClient(apiKey, modelId))
                .ConfigureOptions(x => { x.ModelId = modelId; })
                .UseFunctionInvocation()
                .Build(sp)
                .AsChatCompletionService(sp);
            return skChatService;
        });
        return services;
    }

    public class OpenRouterClient : IChatClient
    {
        private readonly OpenAIClient _client;
        private readonly string _modelId;
        private readonly ChatClientMetadata _metadata;
        private readonly Uri _endpoint;

        public OpenRouterClient(string apiKey, string modelId)
        {
            _endpoint = new Uri("https://openrouter.ai/api/v1");
            var clientOptions = new OpenAIClientOptions()
            {
                Endpoint = _endpoint
            };

            _client = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
            _modelId = modelId;
            _metadata = new ChatClientMetadata("OpenRouter", _endpoint, _modelId);
        }

        public ChatClientMetadata Metadata => _metadata;

        // Changed method name to match interface
        public async Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var chatClient = _client.GetChatClient(_modelId);
                var openAIMessages = OpenRouterMessageConverter.ConvertToOpenAIMessages(messages);
                var chatOptions = OpenRouterOptionsConverter.ConvertToOpenAIOptions(options);

                var response = await chatClient.CompleteChatAsync(
                    openAIMessages,
                    chatOptions,
                    cancellationToken);

                return OpenRouterResponseConverter.ConvertFromOpenAIResponse(response.Value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"OpenRouter API call failed: {ex.Message}", ex);
            }
        }

        // Changed method name and return type to match interface
        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var chatClient = _client.GetChatClient(_modelId);
            var openAIMessages = OpenRouterMessageConverter.ConvertToOpenAIMessages(messages);
            var chatOptions = OpenRouterOptionsConverter.ConvertToOpenAIOptions(options);

            await foreach (var update in chatClient.CompleteChatStreamingAsync(
                               openAIMessages,
                               chatOptions,
                               cancellationToken))
            {
                yield return OpenRouterResponseConverter.ConvertFromStreamingUpdate(update);
            }
        }

        public object GetService(Type serviceType, object serviceKey = null)
        {
            // Return this client if the service type matches
            if (serviceType.IsInstanceOfType(this))
            {
                return this;
            }

            // Return the underlying OpenAI client if requested
            if (serviceType.IsInstanceOfType(_client))
            {
                return _client;
            }

            // Return metadata if requested
            if (serviceType == typeof(ChatClientMetadata))
            {
                return _metadata;
            }

            return null;
        }

        public void Dispose()
        {
            // OpenAIClient doesn't implement IDisposable, so nothing to dispose
        }
    }

    public static class OpenRouterMessageConverter
    {
        public static IList<OpenAI.Chat.ChatMessage> ConvertToOpenAIMessages(
            IEnumerable<ChatMessage> messages)
        {
            var result = new List<OpenAI.Chat.ChatMessage>();

            foreach (var message in messages)
            {
                OpenAI.Chat.ChatMessage openAIMessage = message.Role.Value switch
                {
                    "system" => OpenAI.Chat.ChatMessage.CreateSystemMessage(GetTextContent(message)),
                    "user" => OpenAI.Chat.ChatMessage.CreateUserMessage(ConvertUserContent(message)),
                    "assistant" => OpenAI.Chat.ChatMessage.CreateAssistantMessage(GetTextContent(message)),
                    "tool" => OpenAI.Chat.ChatMessage.CreateToolMessage(
                        GetToolCallId(message),
                        GetTextContent(message)),
                    _ => throw new ArgumentException($"Unsupported role: {message.Role}")
                };

                result.Add(openAIMessage);
            }

            return result;
        }

        private static string GetTextContent(ChatMessage message)
        {
            return message.Text ?? string.Empty;
        }

        private static string GetToolCallId(ChatMessage message)
        {
            // Try to get tool call ID from message properties
            if (message.Contents?.FirstOrDefault() is FunctionCallContent functionCall)
            {
                return functionCall.CallId ?? string.Empty;
            }
            return string.Empty;
        }

        private static IList<OpenAI.Chat.ChatMessageContentPart> ConvertUserContent(ChatMessage message)
        {
            var parts = new List<OpenAI.Chat.ChatMessageContentPart>();

            if (message.Contents != null)
            {
                foreach (var content in message.Contents)
                {
                    switch (content)
                    {
                        case TextContent textContent:
                            parts.Add(OpenAI.Chat.ChatMessageContentPart.CreateTextPart(textContent.Text));
                            break;
                        case DataContent dataContent:
                            if (dataContent.Data.Length != 0)
                            {
                                var imageBytes = dataContent.Data.ToArray();
                                parts.Add(OpenAI.Chat.ChatMessageContentPart.CreateImagePart(
                                    BinaryData.FromBytes(imageBytes), 
                                    dataContent.MediaType));
                            }
                            else if (!string.IsNullOrWhiteSpace(dataContent.Uri))
                            {
                                parts.Add(OpenAI.Chat.ChatMessageContentPart.CreateImagePart(new Uri(dataContent.Uri)));
                            }
                            break;
                    }
                }
            }

            // Fallback to simple text if no content parts
            if (parts.Count == 0 && !string.IsNullOrEmpty(message.Text))
            {
                parts.Add(OpenAI.Chat.ChatMessageContentPart.CreateTextPart(message.Text));
            }

            return parts.Count > 0
                ? parts
                : new List<OpenAI.Chat.ChatMessageContentPart>
                {
                    OpenAI.Chat.ChatMessageContentPart.CreateTextPart(string.Empty)
                };
        }
    }

    public static class OpenRouterOptionsConverter
    {
        public static OpenAI.Chat.ChatCompletionOptions ConvertToOpenAIOptions(ChatOptions options)
        {
            var openAIOptions = new OpenAI.Chat.ChatCompletionOptions();

            if (options == null) return openAIOptions;

            // Map properties directly using the correct property names
            if (options.MaxOutputTokens.HasValue)
                openAIOptions.MaxOutputTokenCount = options.MaxOutputTokens.Value;

            if (options.Temperature.HasValue)
                openAIOptions.Temperature = options.Temperature.Value;

            if (options.TopP.HasValue)
                openAIOptions.TopP = options.TopP.Value;

            if (options.FrequencyPenalty.HasValue)
                openAIOptions.FrequencyPenalty = options.FrequencyPenalty.Value;

            if (options.PresencePenalty.HasValue)
                openAIOptions.PresencePenalty = options.PresencePenalty.Value;

            // Handle stop sequences
            if (options.StopSequences != null)
            {
                foreach (var stop in options.StopSequences)
                {
                    openAIOptions.StopSequences.Add(stop);
                }
            }

            // Handle tools/functions
            if (options.Tools != null)
            {
                foreach (var tool in options.Tools)
                {
                    if (tool is AIFunction aiFunction)
                    {
                        var functionDefinition = OpenAI.Chat.ChatTool.CreateFunctionTool(
                            aiFunction.Name,
                            aiFunction.Description);
                        openAIOptions.Tools.Add(functionDefinition);
                    }
                }
            }

            return openAIOptions;
        }
    }

    public static class OpenRouterResponseConverter
    {
        // Updated to return ChatResponse instead of ChatCompletion
        public static ChatResponse ConvertFromOpenAIResponse(OpenAI.Chat.ChatCompletion response)
        {
            var content = response.Content.FirstOrDefault();
            var messageText = content?.Text ?? string.Empty;

            // Create the message properly
            var message = new ChatMessage(ChatRole.Assistant, messageText);

            // Create usage details if available
            UsageDetails? usage = null;
            if (response.Usage != null)
            {
                usage = new UsageDetails
                {
                    InputTokenCount = response.Usage.InputTokenCount,
                    OutputTokenCount = response.Usage.OutputTokenCount,
                    TotalTokenCount = response.Usage.TotalTokenCount
                };
            }

            // Convert finish reason
            ChatFinishReason? finishReason = ConvertFinishReason(response.FinishReason);

            return new ChatResponse(message)
            {
                CreatedAt = response.CreatedAt,
                ModelId = response.Model,
                FinishReason = finishReason,
                Usage = usage,
                ResponseId = response.Id
            };
        }

        // Updated to return ChatResponseUpdate instead of StreamingChatCompletionUpdate
        public static ChatResponseUpdate ConvertFromStreamingUpdate(
            OpenAI.Chat.StreamingChatCompletionUpdate update)
        {
            var contentUpdate = update.ContentUpdate?.FirstOrDefault();
            var text = contentUpdate?.Text ?? string.Empty;

            ChatFinishReason? finishReason = ConvertFinishReason(update.FinishReason);

            return new ChatResponseUpdate
            {
                Contents = [new TextContent(text)],
                FinishReason = finishReason,
                ResponseId = update.CompletionId
            };
        }

        private static ChatFinishReason? ConvertFinishReason(OpenAI.Chat.ChatFinishReason? finishReason)
        {
            return finishReason switch
            {
                OpenAI.Chat.ChatFinishReason.Stop => ChatFinishReason.Stop,
                OpenAI.Chat.ChatFinishReason.Length => ChatFinishReason.Length,
                OpenAI.Chat.ChatFinishReason.ContentFilter => ChatFinishReason.ContentFilter,
                OpenAI.Chat.ChatFinishReason.ToolCalls => ChatFinishReason.ToolCalls,
                OpenAI.Chat.ChatFinishReason.FunctionCall => ChatFinishReason.ToolCalls,
                _ => null
            };
        }
    }
}
