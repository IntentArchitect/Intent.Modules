using Intent.Modules.Common.AI.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Common.AI.Settings.AISettings;

namespace Intent.Modules.Common.AI.Tasks
{
    /// <summary>
    /// Represents the arguments required to execute an AI prompt task, including application, element, provider, model, and context information.
    /// </summary>
    public class PromptArgs
    {
        /// <summary>
        /// Gets the application ID for the prompt execution.
        /// </summary>
        public string ApplicationId { get; }

        /// <summary>
        /// Gets the element ID for the prompt execution.
        /// </summary>
        public string ElementId { get; }

        /// <summary>
        /// Gets the user-provided context string.
        /// </summary>
        public string UserProvidedContext { get; }

        /// <summary>
        /// Gets the array of example component IDs.
        /// </summary>
        public string[] ExampleComponentIds { get; }

        /// <summary>
        /// Gets the provider option for the prompt execution.
        /// </summary>
        public ProviderOptionsEnum Provider { get; }

        /// <summary>
        /// Gets the model ID for the prompt execution.
        /// </summary>
        public string ModelId { get; }

        /// <summary>
        /// Gets the thinking level for the prompt execution.
        /// </summary>
        public string ThinkingLevel { get; }

        /// <summary>
        /// Gets the template ID for the prompt execution, if provided.
        /// </summary>
        public string? TemplateId { get; }

        private static readonly string[] RequiredKeys =
        {
            "applicationId",
            "elementId",
            "provider",
            "modelId",
            "thinkingLevel"
        };

        private static readonly string[] OptionalKeys =
        {
            "userProvidedContext",
            "exampleComponentIds",
            "templateId"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptArgs"/> class.
        /// </summary>
        /// <param name="applicationId">The application ID.</param>
        /// <param name="elementId">The element ID.</param>
        /// <param name="userProvidedContext">The user-provided context string.</param>
        /// <param name="exampleComponentIds">The array of example component IDs.</param>
        /// <param name="provider">The provider option.</param>
        /// <param name="modelId">The model ID.</param>
        /// <param name="thinkingLevel">The thinking level.</param>
        /// <param name="templateId">The template ID (optional).</param>
        private PromptArgs(
            string applicationId,
            string elementId,
            string userProvidedContext,
            string[] exampleComponentIds,
            ProviderOptionsEnum provider,
            string modelId,
            string thinkingLevel,
            string? templateId)
        {
            ApplicationId = applicationId;
            ElementId = elementId;
            UserProvidedContext = userProvidedContext;
            ExampleComponentIds = exampleComponentIds;
            Provider = provider;
            ModelId = modelId;
            ThinkingLevel = thinkingLevel;
            TemplateId = templateId;
        }

        /// <summary>
        /// Creates a <see cref="PromptArgs"/> instance from a fixed-position argument array.
        /// </summary>
        /// <param name="args">The argument array.</param>
        /// <returns>A <see cref="PromptArgs"/> instance.</returns>
        public static PromptArgs FromArgs(string[] args)
        {
            return FromArgs(args, new Dictionary<string, int>
            {
                ["applicationId"] = 0,
                ["elementId"] = 1,
                ["userProvidedContext"] = 2,
                ["exampleComponentIds"] = 3,
                ["provider"] = 4,
                ["modelId"] = 5,
                ["thinkingLevel"] = 6,
                ["templateId"] = 7
            });
        }

        /// <summary>
        /// Creates a <see cref="PromptArgs"/> instance from an argument array and a key-to-index mapping.
        /// Supports flexible argument ordering and optional keys.
        /// </summary>
        /// <param name="args">The argument array.</param>
        /// <param name="map">A dictionary mapping argument keys to their indices in the array.</param>
        /// <returns>A <see cref="PromptArgs"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> or <paramref name="map"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if a required key is missing in the mapping.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if a required argument index is out of range.</exception>
        public static PromptArgs FromArgs(string[] args, IDictionary<string, int> map)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (map == null) throw new ArgumentNullException(nameof(map));

            // 1. Validate required keys exist in the map
            foreach (var key in RequiredKeys)
            {
                if (!map.ContainsKey(key))
                    throw new ArgumentException($"Argument mapping missing required key '{key}'.");
            }

            // Helper for required fields
            string GetRequired(string key)
            {
                var index = map[key];
                if (index < 0 || index >= args.Length)
                    throw new ArgumentOutOfRangeException($"Index {index} for '{key}' is out of range.");

                return args[index];
            }

            // Helper for optional fields
            string? GetOptional(string key)
            {
                if (!map.TryGetValue(key, out var index))
                    return null; // Key was not provided → treat as missing

                if (index < 0 || index >= args.Length)
                    return null; // Out-of-range → treat as missing

                return args[index];
            }

            // Required
            var applicationId = GetRequired("applicationId");
            var elementId = GetRequired("elementId");
            var providerRaw = GetRequired("provider");
            var modelId = GetRequired("modelId");
            var thinkingLevel = GetRequired("thinkingLevel");

            // Optional
            var rawUserContext = GetOptional("userProvidedContext");
            var userProvidedContext = string.IsNullOrWhiteSpace(rawUserContext)
                ? "None"
                : rawUserContext;

            var rawExamples = GetOptional("exampleComponentIds");
            var exampleComponentIds =
                string.IsNullOrWhiteSpace(rawExamples)
                    ? Array.Empty<string>()
                    : JsonConvert.DeserializeObject<string[]>(rawExamples) ?? Array.Empty<string>();

            var rawTemplate = GetOptional("templateId");
            string? templateId = string.IsNullOrWhiteSpace(rawTemplate) ? null : rawTemplate;

            return new PromptArgs(
                applicationId,
                elementId,
                userProvidedContext,
                exampleComponentIds,
                new AISettings.ProviderOptions(providerRaw).AsEnum(),
                modelId,
                thinkingLevel,
                templateId);
        }
    }
}
