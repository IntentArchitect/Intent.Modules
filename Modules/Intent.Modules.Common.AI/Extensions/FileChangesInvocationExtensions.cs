using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.Exceptions;
using Intent.Modules.Common.AI.CodeGeneration;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Intent.Modules.Common.AI.Extensions;

#nullable enable

/// <summary>
/// Provides extension methods for <see cref="Kernel"/> to simplify AI-driven code generation workflows.
/// </summary>
public static class FileChangesInvocationExtensions
{
    private const int MaxAttempts = 2;
    
    /// <summary>
    /// Executes a prompt and returns file changes, automatically handling function-based
    /// or agent-based execution based on the kernel configuration.
    /// </summary>
    /// <param name="kernel">The Semantic Kernel instance.</param>
    /// <param name="promptTemplate">The prompt template to execute.</param>
    /// <param name="thinkingLevel">The thinking/reasoning level to use (e.g., "none", "low", "medium", "high").</param>
    /// <param name="arguments">Optional kernel arguments to pass to the prompt.</param>
    /// <param name="additionalContent">Optional additional content items (e.g., images, files) to include with the prompt.</param>
    /// <param name="maxAttempts">Maximum number of retry attempts if parsing fails.</param>
    /// <returns>A <see cref="FileChangesResult"/> containing the AI-generated file changes.</returns>
    public static FileChangesResult InvokeFileChangesPrompt(
        this Kernel kernel,
        string promptTemplate,
        string thinkingLevel,
        KernelArguments? arguments = null,
        IReadOnlyList<KernelContent>? additionalContent = null,
        int maxAttempts = MaxAttempts)
    {
        return InvokeFileChangesPromptAsync(kernel, promptTemplate, thinkingLevel, arguments, additionalContent, maxAttempts).GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Executes a prompt and returns file changes, automatically handling function-based
    /// or agent-based execution based on the kernel configuration.
    /// </summary>
    /// <param name="kernel">The Semantic Kernel instance.</param>
    /// <param name="promptTemplate">The prompt template to execute.</param>
    /// <param name="thinkingLevel">The thinking/reasoning level to use (e.g., "none", "low", "medium", "high").</param>
    /// <param name="arguments">Optional kernel arguments to pass to the prompt.</param>
    /// <param name="additionalContent">Optional additional content items (e.g., images, files) to include with the prompt.</param>
    /// <param name="maxAttempts">Maximum number of retry attempts if parsing fails.</param>
    /// <returns>A <see cref="FileChangesResult"/> containing the AI-generated file changes.</returns>
    public static async Task<FileChangesResult> InvokeFileChangesPromptAsync(
        this Kernel kernel,
        string promptTemplate,
        string thinkingLevel,
        KernelArguments? arguments = null,
        IReadOnlyList<KernelContent>? additionalContent = null,
        int maxAttempts = MaxAttempts)
    {
        // Get execution settings for this thinking type
        var aiProviderService = kernel.GetRequiredService<IAiProviderService>();
        var executionSettings = aiProviderService.GetPromptExecutionSettings(thinkingLevel);
        
        return await ExecuteWithRetryAsync(kernel, promptTemplate, executionSettings, arguments, additionalContent, maxAttempts);
    }
    
    private static async Task<FileChangesResult> ExecuteWithRetryAsync(
        Kernel kernel,
        string promptTemplate,
        PromptExecutionSettings executionSettings,
        KernelArguments? arguments,
        IReadOnlyList<KernelContent>? additionalContent,
        int maxAttempts)
    {
        var hasAdditionalContent = additionalContent?.Any() == true;
        var chatCompletionService = hasAdditionalContent ? kernel.GetRequiredService<IChatCompletionService>() : null;
        var kernelFunction = !hasAdditionalContent ? kernel.CreateFunctionFromPrompt(promptTemplate, executionSettings) : null;
        
        FileChangesResult? fileChangesResult = null;
        var previousError = string.Empty;
        
        for (var i = 0; i < maxAttempts; i++)
        {
            var attemptNumber = i + 1;
            Logging.Log.Info($"AI invocation attempt {attemptNumber}/{maxAttempts}");
            
            if (arguments is null)
            {
                arguments = new KernelArguments();
            }
            arguments["previousError"] = previousError;

            if (!arguments.ContainsKey("fileChangesSchema"))
            {
                throw new Exception("Kernel Arguments missing 'fileChangesSchema'. Possible that the prompt template is missing it as well. The AI should be briefed on how the output should be presented. Argument should be initialized with 'FileChangesSchema.GetPromptInstructions()'.");
            }
            
            FunctionResult result;
            try
            {
                if (hasAdditionalContent)
                {
                    // Use chat completion with multimodal content
                    result = await InvokeChatCompletionAsync(kernel, chatCompletionService!, promptTemplate, executionSettings, arguments, additionalContent!);
                }
                else
                {
                    // Use standard function-based approach
                    result = await kernelFunction!.InvokeAsync(kernel, arguments);
                }
            }
            catch (Exception ex)
            {
                var rootException = GetRootException(ex);
                var message = rootException.Message;

                if (message.Contains("reasoning_effort", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FriendlyException(@"The selected model does not have thinking capabilities. Please choose a different model or set the thinking level to 'None'.");
                }

                throw;
            }

            if (FileChangesParser.TryGetFileChangesResult(result, out fileChangesResult, out var errorDetails))
            {
                Logging.Log.Info($"Attempt {attemptNumber} succeeded");
                break;
            }

            // Provide specific error feedback for the retry
            previousError = BuildRetryErrorMessage(errorDetails);
            Logging.Log.Warning($"Attempt {attemptNumber} failed: {errorDetails}");
        }

        if (fileChangesResult is null)
        {
            throw new Exception("AI Prompt failed to return a valid response after all retry attempts.");
        }

        return fileChangesResult;
    }
    
    private static async Task<FunctionResult> InvokeChatCompletionAsync(
        Kernel kernel,
        IChatCompletionService chatCompletionService,
        string promptTemplate,
        PromptExecutionSettings executionSettings,
        KernelArguments arguments,
        IReadOnlyList<KernelContent> additionalContent)
    {
        // Render the prompt template with arguments
        var promptTemplateFactory = new KernelPromptTemplateFactory();
        var renderedPromptTemplate = promptTemplateFactory.Create(new PromptTemplateConfig(promptTemplate));
        var renderedPrompt = await renderedPromptTemplate.RenderAsync(kernel, arguments);
        
        // Build chat history with the rendered prompt and additional content
        var chatHistory = new ChatHistory();
        var messageContent = new ChatMessageContent(AuthorRole.User, []);
        
        // Add the rendered text prompt
        messageContent.Items.Add(new TextContent(renderedPrompt));
        
        // Add additional content items (images, files, etc.)
        foreach (var content in additionalContent)
        {
            if (content is ImageContent imageContent)
            {
                messageContent.Items.Add(imageContent);
            }
            else if (content is TextContent textContent)
            {
                messageContent.Items.Add(textContent);
            }
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only
            else if (content is BinaryContent binaryContent)
            {
                messageContent.Items.Add(binaryContent);
            }
            else if (content is AudioContent audioContent)
            {
                messageContent.Items.Add(audioContent);
            }
#pragma warning restore SKEXP0001
            else
            {
                // For any other KernelContent type, try to add it as-is
                messageContent.Items.Add(content);
            }
        }
        
        chatHistory.Add(messageContent);
        
        // Invoke chat completion - FunctionChoiceBehavior.Auto() in executionSettings 
        // automatically handles tool calling and invocation
        var chatMessageContents = await chatCompletionService.GetChatMessageContentsAsync(
            chatHistory,
            executionSettings,
            kernel);
        
        var result = chatMessageContents.FirstOrDefault() 
            ?? throw new Exception("Chat completion service returned no results.");
        
        // Wrap in FunctionResult for consistent parsing
        var textResult = result.Content ?? string.Empty;
        return new FunctionResult(KernelFunctionFactory.CreateFromMethod(() => textResult));
    }

    private static string BuildRetryErrorMessage(string? errorDetails)
    {
        if (string.IsNullOrWhiteSpace(errorDetails))
        {
            return "The previous prompt execution failed. You need to return ONLY the JSON response in the defined schema format!";
        }

        var message = $"""
                       The previous prompt execution failed with the following error:

                       {errorDetails}

                       """;

        // Add specific guidance based on the error type
        if (errorDetails.Contains("JSON parsing failed", StringComparison.OrdinalIgnoreCase))
        {
            message += """
                       Common JSON errors to avoid:
                       - Trailing commas in arrays or objects
                       - Unescaped quotes or backslashes in strings
                       - Comments (JSON does not support comments)
                       - Single quotes instead of double quotes
                       
                       """;
        }

        message += """
                   Expected schema format:
                   {
                     "FileChanges": [
                       {
                         "FilePath": "path/to/file",
                         "Content": "file content"
                       }
                     ]
                   }
                   """;

        return message;
    }

    private static Exception GetRootException(Exception exception)
    {
        if (exception is AggregateException aggregateException)
        {
            var flattened = aggregateException.Flatten();
            if (flattened.InnerExceptions.Count == 1)
            {
                return GetRootException(flattened.InnerExceptions[0]);
            }

            return flattened;
        }

        return exception.InnerException is null ? exception : GetRootException(exception.InnerException);
    }
}
