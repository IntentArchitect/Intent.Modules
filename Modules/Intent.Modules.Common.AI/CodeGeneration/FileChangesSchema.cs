namespace Intent.Modules.Common.AI.CodeGeneration;

#nullable enable

/// <summary>
/// Defines the JSON schema and prompt instructions for AI-generated file changes.
/// This ensures the prompt and parser always stay in sync.
/// </summary>
public static class FileChangesSchema
{
    /// <summary>
    /// JSON Schema definition for AI responses containing file changes.
    /// </summary>
    public static string JsonSchema => """
        {
            "type": "object",
            "properties": {
                "FileChanges": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "FilePath": { "type": "string" },
                            "FileExplanation": { "type": "string" },
                            "Content": { "type": "string" }
                        },
                        "required": ["FilePath", "Content"],
                        "additionalProperties": false
                    }
                }
            },
            "required": ["FileChanges"],
            "additionalProperties": false
        }
        """;

    /// <summary>
    /// Gets the complete prompt instructions section for file changes output format.
    /// This should be included in AI prompts that expect FileChangesResult responses.
    /// </summary>
    public static string GetPromptInstructions()
    {
        // Use $$$" to allow {{$variable}} template placeholders
        return $$$"""
            ## Required Output Format
            Your response MUST include:
            1. Respond ONLY with deserializable JSON that matches the following schema:
            {{{JsonSchema}}}
            
            EXAMPLE RESPONSE BEGIN
            {
                "FileChanges": [
                    {
                        "FilePath": <some-file-path_1>,
                        "Content": <some-file-content_1>
                    },
                    {
                        "FilePath": <some-file-path_2>,
                        "Content": <some-file-content_2>
                    }
                ]
            }
            EXAMPLE RESPONSE END
            """;
    }
}
