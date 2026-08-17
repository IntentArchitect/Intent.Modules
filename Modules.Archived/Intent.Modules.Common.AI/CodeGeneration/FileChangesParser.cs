using System;
using System.Diagnostics.CodeAnalysis;
using Intent.Modules.Common.AI.CodeGeneration.Models;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Intent.Modules.Common.AI.CodeGeneration;

#nullable enable

/// <summary>
/// Provides utilities for parsing AI-generated file changes from Semantic Kernel function results.
/// </summary>
public static class FileChangesParser
{
    /// <summary>
    /// Attempts to parse a <see cref="FunctionResult"/> into a <see cref="FileChangesResult"/>.
    /// AI Models / APIs that don't support JSON Schema Formatting (like OpenAI) will not completely give you a "clean result".
    /// This will attempt to cut through the thoughts it writes out to get to the payload.
    /// </summary>
    /// <param name="aiInvocationResponse">The function result from an AI invocation.</param>
    /// <param name="fileChanges">The parsed file changes result if successful.</param>
    /// <param name="errorDetails">Error details if parsing fails.</param>
    /// <returns><c>true</c> if parsing was successful; otherwise, <c>false</c>.</returns>
    public static bool TryGetFileChangesResult(FunctionResult aiInvocationResponse, [NotNullWhen(true)] out FileChangesResult? fileChanges, out string? errorDetails)
    {
        try
        {
            var textResponse = aiInvocationResponse.ToString();
            string payload;
            var jsonMarkdownStart = textResponse.IndexOf("```", StringComparison.Ordinal);
            if (jsonMarkdownStart < 0)
            {
                // Assume AI didn't respond with ``` wrappers.
                // JSON Deserializer will pick up if it is not valid.
                payload = textResponse;
            }
            else
            {
                var sanitized = textResponse.Substring(jsonMarkdownStart).Replace("```json", "").Replace("```", "");
                payload = sanitized;
            }

            var fileChangesResult = JsonConvert.DeserializeObject<FileChangesResult>(payload);
            if (fileChangesResult is null)
            {
                fileChanges = null;
                errorDetails = "JSON deserialization returned null. Response may be empty or invalid.";
                return false;
            }

            fileChanges = fileChangesResult;
            errorDetails = null;
            return true;
        }
        catch (JsonException jsonEx)
        {
            fileChanges = null;
            errorDetails = $"JSON parsing failed: {jsonEx.Message}";
            return false;
        }
        catch (Exception ex)
        {
            fileChanges = null;
            errorDetails = $"Unexpected error: {ex.Message}";
            return false;
        }
    }
}

/// <summary>
/// Represents the result of an AI-generated file changes operation.
/// </summary>
public class FileChangesResult
{
    /// <summary>
    /// Gets or sets the array of file changes.
    /// </summary>
    public FileChange[] FileChanges { get; set; } = [];
}