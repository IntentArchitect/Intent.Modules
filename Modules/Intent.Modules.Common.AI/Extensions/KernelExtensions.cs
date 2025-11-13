using System;
using System.Threading.Tasks;
using Intent.Exceptions;
using Intent.Modules.Common.AI.CodeGeneration;
using Intent.Modules.Common.AI.CodeGeneration.Models;
using Intent.Utils;
using Microsoft.SemanticKernel;

namespace Intent.Modules.Common.AI.Extensions;

#nullable enable

/// <summary>
/// Provides extension methods for <see cref="Kernel"/> to simplify AI-driven code generation workflows.
/// </summary>
public static class KernelExtensions
{
    private const int MaxAttempts = 2;
    
    /// <summary>
    /// Executes a prompt and returns file changes, automatically handling function-based
    /// or agent-based execution based on the kernel configuration.
    /// </summary>
    /// <param name="kernel">The Semantic Kernel instance.</param>
    /// <param name="promptTemplate">The prompt template to execute.</param>
    /// <param name="thinkingType">The thinking/reasoning level to use (e.g., "none", "low", "medium", "high").</param>
    /// <param name="arguments">Optional kernel arguments to pass to the prompt.</param>
    /// <param name="maxAttempts">Maximum number of retry attempts if parsing fails.</param>
    /// <returns>A <see cref="FileChangesResult"/> containing the AI-generated file changes.</returns>
    public static async Task<FileChangesResult> InvokeFileChangesPromptAsync(
        this Kernel kernel,
        string promptTemplate,
        string thinkingType,
        KernelArguments? arguments = null,
        int maxAttempts = MaxAttempts)
    {
        // Get execution settings for this thinking type
        var aiProviderService = kernel.GetRequiredService<IAiProviderService>();
        var executionSettings = aiProviderService.GetPromptExecutionSettings(thinkingType);
        
        // Create the kernel function from the prompt template
        var kernelFunction = kernel.CreateFunctionFromPrompt(promptTemplate, executionSettings);
        
        // Execute with retry logic
        return await ExecuteWithFunctionAsync(kernelFunction, kernel, arguments, maxAttempts);
    }

    /// <summary>
    /// Executes a prompt and returns file changes, automatically handling function-based
    /// or agent-based execution based on the kernel configuration.
    /// </summary>
    /// <param name="kernel">The Semantic Kernel instance.</param>
    /// <param name="promptTemplate">The prompt template to execute.</param>
    /// <param name="thinkingType">The thinking/reasoning level to use (e.g., "none", "low", "medium", "high").</param>
    /// <param name="arguments">Optional kernel arguments to pass to the prompt.</param>
    /// <param name="maxAttempts">Maximum number of retry attempts if parsing fails.</param>
    /// <returns>A <see cref="FileChangesResult"/> containing the AI-generated file changes.</returns>
    public static FileChangesResult InvokeFileChangesPrompt(
        this Kernel kernel,
        string promptTemplate,
        string thinkingType,
        KernelArguments? arguments = null,
        int maxAttempts = MaxAttempts)
    {
        return InvokeFileChangesPromptAsync(kernel, promptTemplate, thinkingType, arguments, maxAttempts).GetAwaiter().GetResult();
    }
    
    private static async Task<FileChangesResult> ExecuteWithFunctionAsync(
        KernelFunction kernelFunction,
        Kernel kernel,
        KernelArguments? arguments,
        int maxAttempts)
    {
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
                result = await kernelFunction.InvokeAsync(kernel, arguments);
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
