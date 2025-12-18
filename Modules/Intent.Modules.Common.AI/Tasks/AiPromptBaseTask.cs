using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.AI.CodeGeneration;
using Intent.Modules.Common.AI.Configuration;
using Intent.Modules.Common.AI.Extensions;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intent.Modules.Common.AI.Tasks
{
    /// <summary>
    /// Provides a base implementation for AI prompt tasks, handling prompt configuration, input resolution,
    /// kernel invocation, and file change application.
    /// </summary>
    /// <typeparam name="TPromptInputs">The type representing prompt inputs.</typeparam>
    public abstract class AiPromptBaseTask<TPromptInputs> : IModuleTask
    {
        private readonly IOutputRegistry _outputRegistry;
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
        private readonly IGeneratedFilesProvider _fileProvider;
        private readonly IMetadataManager _metadataManager;
        private readonly ISolutionConfig _solution;
        private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

        /// <summary>
        /// Gets the unique identifier for the task type.
        /// </summary>
        public string TaskTypeId { get; }

        /// <summary>
        /// Gets the name of the task type.
        /// </summary>
        public abstract string TaskTypeName { get; }

        /// <summary>
        /// Gets the execution order for the task. Default is 0.
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AiPromptBaseTask{TPromptInputs}"/> class.
        /// </summary>
        /// <param name="taskTypeId">The unique identifier for the task type.</param>
        /// <param name="applicationConfigurationProvider">The application configuration provider.</param>
        /// <param name="metadataManager">The metadata manager.</param>
        /// <param name="solution">The solution configuration.</param>
        /// <param name="outputRegistry">The output registry for file changes.</param>
        /// <param name="userSettingsProvider">The user settings provider.</param>
        protected AiPromptBaseTask(string taskTypeId,
            IApplicationConfigurationProvider applicationConfigurationProvider,
            IMetadataManager metadataManager,
            ISolutionConfig solution,
            IOutputRegistry outputRegistry,
            IUserSettingsProvider userSettingsProvider)
        {
            TaskTypeId = taskTypeId;
            _outputRegistry = outputRegistry;
            _applicationConfigurationProvider = applicationConfigurationProvider;
            _metadataManager = metadataManager;
            _solution = solution;
            _intentSemanticKernelFactory = new IntentSemanticKernelFactory(userSettingsProvider);
            _fileProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        }

        /// <summary>
        /// Loads prompt arguments from the specified argument array.
        /// </summary>
        /// <param name="args">The arguments to load.</param>
        /// <returns>A <see cref="PromptArgs"/> instance.</returns>
        protected virtual PromptArgs LoadArgs(params string[] args)
        {
            return PromptArgs.FromArgs(args);
        }

        /// <summary>
        /// Gets the designer for the specified application ID.
        /// </summary>
        /// <param name="applicationId">The application ID.</param>
        /// <returns>The designer instance.</returns>
        protected abstract IDesigner GetDesigner(string applicationId);

        /// <summary>
        /// Creates kernel arguments from the specified prompt inputs.
        /// </summary>
        /// <param name="inputs">The prompt inputs.</param>
        /// <returns>A <see cref="KernelArguments"/> instance.</returns>
        protected abstract KernelArguments CreateKernelArguments(TPromptInputs inputs);

        /// <summary>
        /// Builds the prompt inputs from the provided execution arguments, component model, prompt configuration, and prompt context.
        /// </summary>
        /// <param name="execArgs">The execution arguments.</param>
        /// <param name="componentModel">The component model element.</param>
        /// <param name="promptTemplateConfig">The prompt configuration.</param>
        /// <param name="promptContext">The prompt context.</param>
        /// <returns>The constructed prompt inputs.</returns>
        protected abstract TPromptInputs BuildPromptInputs(PromptArgs execArgs,
            IElement componentModel,
            PromptConfig promptTemplateConfig,
            PromptContext promptContext);

        /// <summary>
        /// Executes the AI prompt task using the provided arguments.
        /// </summary>
        /// <param name="args">The arguments for execution.</param>
        /// <returns>A string indicating the result of execution.</returns>
        public string Execute(params string[] args)
        {
            var execArgs = LoadArgs(args);
            Logging.Log.Info($"Args: {string.Join(",", args)}");

            var applicationConfig = _solution.GetApplicationConfig(execArgs.ApplicationId);
            var designer = GetDesigner(execArgs.ApplicationId);

            var kernel = _intentSemanticKernelFactory.BuildSemanticKernel(
                execArgs.ModelId,
                execArgs.Provider,
                null);

            var componentModel = GetComponentModel(designer, execArgs.ElementId);

            var promptTemplateConfig = PromptConfig.Load(
                _solution.SolutionRootLocation,
                applicationConfig.Name,
                TaskTypeId,
                execArgs.TemplateId);

            // Setup MCP Servers if any are configured
            using var mcpScope = McpScope.Create(kernel, promptTemplateConfig.McpServers);

            var fileContext = promptTemplateConfig.GetFileContext(_applicationConfigurationProvider, _fileProvider);

            var promptContext = new PromptContext(
                GetUserDefinedContext(execArgs, componentModel),
                promptTemplateConfig.GetMetadata(execArgs.TemplateId),
                promptTemplateConfig.GetInputFiles(fileContext, execArgs.TemplateId),
                GetExampleFiles(designer, execArgs.ExampleComponentIds),
                promptTemplateConfig.LoadTemplateFiles(execArgs.TemplateId),
                promptTemplateConfig.GetAdditionalRules(execArgs.TemplateId)
                );

            var promptInputs = BuildPromptInputs(
                execArgs,
                componentModel,
                promptTemplateConfig,
                promptContext);

            var kernelArguments = CreateKernelArguments(promptInputs);

            var promptTemplate = promptTemplateConfig.GetPromptTemplate([.. kernelArguments.Keys]);

            var fileChangesResult = kernel.InvokeFileChangesPrompt(
                promptTemplate: promptTemplate,
                thinkingLevel: execArgs.ThinkingLevel,
                arguments: kernelArguments);

            ApplyFileChanges(applicationConfig, fileChangesResult);

            return "success";
        }

        /// <summary>
        /// Gets the user-defined context string, combining user-provided context and component model comments.
        /// </summary>
        /// <param name="execArgs">The execution arguments.</param>
        /// <param name="componentModel">The component model element.</param>
        /// <returns>The combined user-defined context string.</returns>
        protected virtual string GetUserDefinedContext(PromptArgs execArgs, IElement componentModel)
        {
            var userProvidedContext = string.IsNullOrWhiteSpace(execArgs.UserProvidedContext)
                ? "None"
                : execArgs.UserProvidedContext;

            if (!string.IsNullOrEmpty(componentModel.Comment))
            {
                userProvidedContext = componentModel.Comment + Environment.NewLine + userProvidedContext;
            }

            return userProvidedContext;
        }

        /// <summary>
        /// Gets the environment metadata as a JSON string, merging the application configuration and prompt template metadata.
        /// </summary>
        /// <param name="applicationConfig">The application configuration.</param>
        /// <param name="promptTempalteMetadata">The prompt template metadata.</param>
        /// <returns>The merged metadata as a JSON string.</returns>
        protected string GetEnvironmentMetadata(IApplicationConfig applicationConfig, Dictionary<string, object> promptTempalteMetadata)
        {
            // Build a dictionary so values can be easily swapped or extended
            var metadata = new Dictionary<string, object>
            {
            };

            metadata = DictionaryHelper.MergeDictionaries(metadata, promptTempalteMetadata);

            return JsonConvert.SerializeObject(metadata, Formatting.Indented);
        }

        /// <summary>
        /// Gets the component model element for the specified designer and element ID.
        /// </summary>
        /// <param name="designer">The designer instance.</param>
        /// <param name="elementId">The element ID.</param>
        /// <returns>The component model element.</returns>
        /// <exception cref="Exception">Thrown if the element cannot be found.</exception>
        protected IElement GetComponentModel(IDesigner designer, string elementId)
        {
            var componentModel = designer.Elements.FirstOrDefault(x => x.Id == elementId);

            if (componentModel == null)
            {
                throw new Exception(
                    "An element was selected to be executed upon but could not be found. " +
                    "Please ensure you have saved your designer and try again.");
            }

            return componentModel;
        }

        /// <summary>
        /// Gets example files for the specified designer and example component IDs.
        /// </summary>
        /// <param name="designer">The designer instance.</param>
        /// <param name="exampleComponentIds">The array of example component IDs.</param>
        /// <returns>A list of codebase files representing the example files.</returns>
        protected List<ICodebaseFile> GetExampleFiles(
            IDesigner designer,
            string[] exampleComponentIds)
        {
            if (exampleComponentIds is null || exampleComponentIds.Length == 0)
            {
                return new List<ICodebaseFile>();
            }

            return exampleComponentIds
                .SelectMany(componentId =>
                {
                    var component = designer.Elements.FirstOrDefault(x => x.Id == componentId);
                    return component == null
                        ? Array.Empty<ICodebaseFile>()
                        : _fileProvider.GetFilesForMetadata(component);
                })
                .ToList();
        }

        /// <summary>
        /// Applies file changes from the result to the output registry, using the specified application configuration.
        /// </summary>
        /// <param name="applicationConfig">The application configuration.</param>
        /// <param name="fileChangesResult">The result containing file changes.</param>
        protected void ApplyFileChanges(IApplicationConfig applicationConfig, FileChangesResult fileChangesResult)
        {
            var basePath = Path.GetFullPath(
                Path.Combine(applicationConfig.DirectoryPath.Replace("\\", "/"), applicationConfig.OutputLocation.Replace("\\", "/")));

            foreach (var fileChange in fileChangesResult.FileChanges)
            {
                var fullPath = Path.Combine(basePath, fileChange.FilePath);
                _outputRegistry.Register(fullPath, fileChange.Content);
            }
        }
    }
}
