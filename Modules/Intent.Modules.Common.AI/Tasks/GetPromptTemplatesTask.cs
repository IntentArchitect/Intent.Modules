using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.AI.Configuration;
using Intent.Plugins;
using Newtonsoft.Json;

namespace Intent.Modules.Common.AI.Tasks
{
    /// <summary>
    /// Retrieves available AI prompt templates for a given application and accelerator task.
    /// Returns a JSON array of template results, including recommended defaults based on concept matching.
    /// </summary>
    public class GetPromptTemplatesTask : IModuleTask
	{
        private readonly ISolutionConfig _solutionConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPromptTemplatesTask"/> class.
        /// </summary>
        /// <param name="solutionConfig">The solution configuration provider.</param>
        public GetPromptTemplatesTask(ISolutionConfig solutionConfig)
        {
            _solutionConfig = solutionConfig;
        }

        /// <summary>
        /// Gets the unique task type identifier.
        /// </summary>
        public string TaskTypeId => "Intent.Modules.Common.AI.Tasks.GetPromptTemplates";

        /// <summary>
        /// Gets the display name for the task type.
        /// </summary>
        public string TaskTypeName => "Get AI Prompt Templates For AI Accelerator";

        /// <summary>
        /// Gets the execution order for the task.
        /// </summary>
        public int Order => 0;

        /// <summary>
        /// Executes the task to retrieve prompt templates for the specified application, accelerator task, and concept name.
        /// </summary>
        /// <param name="args">
        /// <list type="number">
        /// <item><description>Application Id</description></item>
        /// <item><description>AI Accelerator TaskTypeId</description></item>
        /// <item><description>Concept Name</description></item>
        /// </list>
        /// </param>
        /// <returns>A JSON string representing the available prompt templates and their metadata.</returns>
        /// <exception cref="Exception">Thrown if required arguments are missing.</exception>
        public string Execute(params string[] args)
        {
            string applicationId = args.Length > 0 ? args[0] : throw new Exception($"No Application Id specified for {TaskTypeId}");
            string promptTaskId = args.Length > 1 ? args[1] : throw new Exception($"No AI Accelerator TaskTypeId specified");
            string conceptName = args.Length > 2 ? args[2] : throw new Exception($"No ConceptName Name specified for {TaskTypeId}");

            var applicationConfig = _solutionConfig.GetApplicationConfig(applicationId);

            var result = new List<TemplateResult>();
            var config = PromptConfig.Load(_solutionConfig.SolutionRootLocation, applicationConfig.Name, promptTaskId);
            if (config != null)
            {
                var guess = TemplateGuesser.Guess(config, conceptName);
                result.AddRange(config.Templates.Select(t => new TemplateResult
                {
                    Id = t.Id,
                    Description = t.Name,
                    AdditionalInfo = t.Description,
                    DefaultUserPrompt = t.DefaultUserPrompt,
                    RecommenedDefault = guess is null ? false : t.Id == guess.TemplateId
                }));
                result.Insert(0, new TemplateResult { Id = "", Description = "None", RecommenedDefault = guess is null ? true : false });
            }
            return JsonConvert.SerializeObject(result, JsonNet.Settings);
        }
    }

    /// <summary>
    /// Represents the result for a single prompt template, including metadata and recommendation status.
    /// </summary>
    public class TemplateResult
    {
        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the template name or description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets additional information about the template.
        /// </summary>
        public string AdditionalInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this template is recommended as the default.
        /// </summary>
        public bool RecommenedDefault { get; set; }

        /// <summary>
        /// Gets or sets the default user prompt for the template.
        /// </summary>
        public string DefaultUserPrompt { get; set; }
    }
}
