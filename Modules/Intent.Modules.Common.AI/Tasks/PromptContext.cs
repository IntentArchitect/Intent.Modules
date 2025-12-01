using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Tasks
{
    /// <summary>
    /// Encapsulates the context for an AI prompt, including user-provided context, metadata, code files, example files, template files, and additional rules.
    /// </summary>
    public class PromptContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptContext"/> class.
        /// </summary>
        /// <param name="userProvidedContext">The user-provided context string.</param>
        /// <param name="metadata">The metadata dictionary for the prompt.</param>
        /// <param name="includeCodeFiles">The code files to include in the prompt context.</param>
        /// <param name="exampleFiles">The example files to include in the prompt context.</param>
        /// <param name="templateFiles">The template files to include in the prompt context.</param>
        /// <param name="additionalRules">Additional rules to include in the prompt context.</param>
        public PromptContext(
            string userProvidedContext,
            Dictionary<string, object> metadata,
            IEnumerable<ICodebaseFile> includeCodeFiles,
            IEnumerable<ICodebaseFile> exampleFiles,
            IEnumerable<ICodebaseFile> templateFiles,
            string additionalRules)
        {
            UserProvidedContext = userProvidedContext;
            Metadata = metadata;
            IncludeCodeFiles = includeCodeFiles;
            ExampleFiles = exampleFiles;
            TemplateFiles = templateFiles;
            AdditionalRules = additionalRules;
        }

        /// <summary>
        /// Gets the user-provided context string.
        /// </summary>
        public string UserProvidedContext { get; }

        /// <summary>
        /// Gets the code files to include in the prompt context.
        /// </summary>
        public IEnumerable<ICodebaseFile> IncludeCodeFiles { get; }

        /// <summary>
        /// Gets the example files to include in the prompt context.
        /// </summary>
        public IEnumerable<ICodebaseFile> ExampleFiles { get; }

        /// <summary>
        /// Gets the template files to include in the prompt context.
        /// </summary>
        public IEnumerable<ICodebaseFile> TemplateFiles { get; }

        /// <summary>
        /// Gets the additional rules to include in the prompt context.
        /// </summary>
        public string? AdditionalRules { get; }

        /// <summary>
        /// Gets the metadata dictionary for the prompt.
        /// </summary>
        public Dictionary<string, object> Metadata { get; }
    }
}
