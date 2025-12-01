using Anthropic.SDK.Messaging;
using Intent.Configuration;
using Intent.Engine;
using Intent.Templates;
using Intent.Utils;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    /// <summary>
    /// Represents the configuration for an AI prompt, including metadata, rules, input files, templates, and MCP servers.
    /// </summary>
    public class PromptConfig
    {
        /// <summary>
        /// The default configuration file name for prompt configuration.
        /// </summary>
        internal const string DefaultConfigFileName = "prompt-config.json";

        [JsonIgnore]
        private string _promptTemplate { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate required prompt arguments.
        /// </summary>
        [JsonPropertyName("validate-prompt-args")]
        public bool ValidatePromptArgs { get; set; } = true;

        /// <summary>
        /// Gets or sets the metadata associated with the prompt.
        /// </summary>
        [JsonPropertyName("metadata")]
        [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the list of rules for the prompt.
        /// </summary>
        [JsonPropertyName("rules")]
        public List<string> Rules { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of input files for the prompt.
        /// </summary>
        [JsonPropertyName("input-files")]
        public List<InputFile> InputFiles { get; set; } = new List<InputFile>();

        /// <summary>
        /// Gets or sets the list of templates associated with the prompt.
        /// </summary>
        [JsonPropertyName("templates")]
        public List<Template> Templates { get; set; } = new List<Template>();

        /// <summary>
        /// Gets or sets the list of MCP servers for the prompt.
        /// </summary>
        [JsonPropertyName("mcp-servers")]
        public List<McpServer> McpServers { get; set; } = new();

        /// <summary>
        /// Gets the path to the template prompt file for the specified solution, application, and prompt name.
        /// </summary>
        /// <param name="solutionPath">The solution directory path.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="promptName">The prompt name.</param>
        /// <returns>The full path to the template prompt file.</returns>
        public static string GetTemplatePromptPath(string solutionPath, string applicationName, string promptName)
        { 
            return Path.Combine(solutionPath, "AI.Prompt.Templates", applicationName, promptName);
        }

        /// <summary>
        /// Loads a <see cref="PromptConfig"/> from the specified solution, application, and prompt name.
        /// Optionally validates the existence of a template ID.
        /// </summary>
        /// <param name="solutionPath">The solution directory path.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="promptName">The prompt name.</param>
        /// <param name="templateId">The template ID to validate (optional).</param>
        /// <returns>The loaded <see cref="PromptConfig"/> instance.</returns>
        /// <exception cref="Exception">Thrown if the prompt template file is not found or the template ID is invalid.</exception>
        public static PromptConfig Load(string solutionPath, string applicationName, string promptName, string templateId = null)
        {
            var path = GetTemplatePromptPath(solutionPath, applicationName, promptName);
            var filename = System.IO.Directory.Exists(path) ? Path.Combine(path, DefaultConfigFileName) : path;
            if (System.IO.File.Exists(filename))
            {
                var result = Load(filename);

                //If there is a templateId make sure it exists
                result.ValidateTemplateId(templateId);

                return result;
            }
            throw new Exception($"No Prompt Templates Found. ({filename}), running the Software Factory may create these files." ) ;
        }

        /// <summary>
        /// Loads a <see cref="PromptConfig"/> from the specified configuration file path.
        /// </summary>
        /// <param name="configFilePath">The path to the configuration file.</param>
        /// <returns>The loaded <see cref="PromptConfig"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the configuration deserializes to null.</exception>
        public static PromptConfig Load(string configFilePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            options.Converters.Add(new DictionaryStringObjectJsonConverter());

            string json = System.IO.File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize<PromptConfig>(json, options)
                         ?? throw new InvalidOperationException("Config deserialized to null.");

            // Store the config file directory in memory for later path resolution (if you want)
            config.FilePath = Path.GetDirectoryName(Path.GetFullPath(configFilePath))!;
            string promptTempaltFile = Path.Combine( Path.GetDirectoryName(Path.GetFullPath(configFilePath))!, "prompt.md");
            config._promptTemplate = System.IO.File.ReadAllText(promptTempaltFile);

            return config;
        }

        /// <summary>
        /// Gets the merged metadata for the specified template ID and additional metadata.
        /// </summary>
        /// <param name="templateId">The template ID to use for merging metadata.</param>
        /// <param name="additionalMetadata">Additional metadata to merge (optional).</param>
        /// <returns>The merged metadata dictionary.</returns>
        public Dictionary<string, object> GetMetadata(string? templateId, Dictionary<string, object> additionalMetadata = null)
        {
            Dictionary<string, object> result;
            var template = Templates.FirstOrDefault(t => t.Id == templateId);
            if (template != null)
            { 
                result = DictionaryHelper.MergeDictionaries(Metadata, template.Metadata);
            }
            else
            {
                result = Metadata;
            }
            if (additionalMetadata != null)
            {
                result = DictionaryHelper.MergeDictionaries(additionalMetadata, result);
            }
            return result;
        }

        /// <summary>
        /// Gets the input files for the prompt, resolving their locations using the provided <see cref="FileContext"/>.
        /// </summary>
        /// <param name="fileContext">The file context for resolving file locations.</param>
        /// <param name="templateId">The template ID to use for input files (optional).</param>
        /// <returns>An enumerable of <see cref="ICodebaseFile"/> representing the input files.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileContext"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if an input file is not found.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if an input folder is not found.</exception>
        public IEnumerable<ICodebaseFile> GetInputFiles(FileContext fileContext, string? templateId)
        {
            if (fileContext == null) throw new ArgumentNullException(nameof(fileContext));

            IEnumerable<InputFile> inputs;
            var template = Templates.FirstOrDefault(t => t.Id == templateId);
            if (string.IsNullOrWhiteSpace(templateId) || template == null)
            {
                inputs = InputFiles.Select(f => f);
            }
            else
            {
                inputs = InputFiles.Select(f => f).Concat(template.InputFiles.Select(f => f));
            }

            foreach (var input in inputs)
            {
                switch (input.Type)
                {
                    case InputFileType.File:
                        {
                            var raw = input.FileLocationAndName?.Replace("\\", "/") ?? input.Filename?.Replace("\\", "/");
                            if (string.IsNullOrWhiteSpace(raw))
                            {
                                continue;
                            }

                            var path = fileContext.ResolveLocation(raw);

                            if (!System.IO.File.Exists(path))
                            {
                                throw new System.IO.FileNotFoundException(
                                    $"Input file not found: {path}", path);
                            }

                            yield return new FileDetails(System.IO.File.ReadAllText(path));
                            break;
                        }

                    case InputFileType.Folder:
                        {
                            string folderLoction = input.FolderLocation?.Replace("\\", "/");
                            if (string.IsNullOrWhiteSpace(folderLoction))
                            {
                                continue;
                            }

                            var folder = fileContext.ResolveLocation(folderLoction);

                            if (!System.IO.Directory.Exists(folder))
                            {
                                throw new System.IO.DirectoryNotFoundException(
                                    $"Input folder not found: {folder}");
                            }

                            var files = System.IO.Directory.GetFiles(
                                folder,
                                "*.*",
                                System.IO.SearchOption.TopDirectoryOnly);

                            foreach (var filePath in files)
                            {
                                yield return new FileDetails(System.IO.File.ReadAllText(filePath));
                            }

                            break;
                        }

                    case InputFileType.Template:
                        {
                            if (string.IsNullOrWhiteSpace(input.TemplateId) ||
                                fileContext.TemplateResolver == null)
                            {
                                continue;
                            }

                            foreach (var file in fileContext.TemplateResolver(input.TemplateId))
                            {
                                yield return file;
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Loads the template files for the specified template ID.
        /// </summary>
        /// <param name="templateId">The template ID to load files for.</param>
        /// <returns>An enumerable of <see cref="ICodebaseFile"/> representing the template files.</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the template folder is not found.</exception>
        public IEnumerable<ICodebaseFile> LoadTemplateFiles(string templateId)
        {
            var template = Templates.FirstOrDefault(t => t.Id == templateId);
            if (string.IsNullOrEmpty(FilePath) || template is null)
            {
                return [];
            }
            var folderPath = Path.Combine(FilePath, template.TemplateFolder.Replace("\\", "/"));

            if (!System.IO.Directory.Exists(folderPath))
                throw new System.IO.DirectoryNotFoundException($"Template folder not found: {folderPath}");

            var files = System.IO.Directory.GetFiles(folderPath, "*.*", System.IO.SearchOption.TopDirectoryOnly);

            return files.Select(f => new FileDetails(System.IO.File.ReadAllText(f))).ToList();
        }

        /// <summary>
        /// Gets the additional rules for the specified template ID, including both global and template-specific rules.
        /// </summary>
        /// <param name="templateId">The template ID to get rules for.</param>
        /// <returns>A string containing the additional rules.</returns>
        public string GetAdditionalRules(string templateId)
        {
            var result = new StringBuilder();
            var template = Templates.FirstOrDefault(t => t.Id == templateId);

            foreach (var rule in Rules)
            {
                result.AppendLine($"* {rule}");
            }
            if (template is not null)
            {
                foreach (var rule in template.Rules)
                {
                    result.AppendLine($"* {rule}");
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets the prompt template, validating that all required variables are present if validation is enabled.
        /// </summary>
        /// <param name="requiredVariables">The list of required variable names.</param>
        /// <returns>The prompt template string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the prompt template or required variables are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if required variables are missing from the template.</exception>
        public string GetPromptTemplate(IList<string> requiredVariables)
        {

            if (string.IsNullOrWhiteSpace(_promptTemplate))
                throw new ArgumentNullException(nameof(_promptTemplate));

            if (requiredVariables == null)
                throw new ArgumentNullException(nameof(requiredVariables));

            if (ValidatePromptArgs)
            {
                var missing = new List<string>();

                foreach (var variable in requiredVariables)
                {
                    var token = "{{$" + variable + "}}";

                    if (!_promptTemplate.Contains(token, StringComparison.Ordinal))
                    {
                        missing.Add(variable);
                    }
                }

                if (missing.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"Prompt template is missing required variables: {string.Join(", ", missing)}");
                }
            }
            return _promptTemplate;
        }

        /// <summary>
        /// Gets a <see cref="FileContext"/> for resolving file locations, using the provided application configuration and file provider.
        /// </summary>
        /// <param name="applicationConfigurationProvider">The application configuration provider.</param>
        /// <param name="_fileProvider">The generated files provider.</param>
        /// <returns>A configured <see cref="FileContext"/> instance.</returns>
        public FileContext GetFileContext(IApplicationConfigurationProvider applicationConfigurationProvider, IGeneratedFilesProvider _fileProvider)
        {
            var applicationConfig = applicationConfigurationProvider.GetApplicationConfig();
            var locationVariables = new Dictionary<string, string>
            {
                ["applicationLocation"] = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath.Replace("\\", "/"), applicationConfig.OutputLocation.Replace("\\", "/")))
            };                                
            return new FileContext(this.FilePath, locationVariables, _fileProvider.GetFilesForTemplate);
        }

        /// <summary>
        /// Validates that the specified template ID exists in the list of templates.
        /// </summary>
        /// <param name="templateId">The template ID to validate.</param>
        /// <exception cref="Exception">Thrown if the template ID is not found.</exception>
        private void ValidateTemplateId(string templateId)
        {
            if (templateId is not null &&
                Templates.All(t => t.Id != templateId))
            {
                throw new Exception($"Could not find Template {templateId}.");
            }
        }
    }

    /// <summary>
    /// Specifies the type of input file for a prompt.
    /// </summary>
    public enum InputFileType
    {
        /// <summary>
        /// A regular file.
        /// </summary>
        File,
        /// <summary>
        /// A folder containing files.
        /// </summary>
        Folder,
        /// <summary>
        /// A template input.
        /// </summary>
        Template
    }

    /// <summary>
    /// Represents an input file configuration for a prompt.
    /// </summary>
    public class InputFile
    {
        /// <summary>
        /// Gets or sets the type of input file.
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public InputFileType Type { get; set; } = InputFileType.File;

        /// <summary>
        /// Gets or sets the file location and name for file input type.
        /// </summary>
        [JsonPropertyName("file-location")]
        public string? FileLocationAndName { get; set; }

        /// <summary>
        /// Gets or sets the folder location for folder input type.
        /// </summary>
        [JsonPropertyName("folder-location")]
        public string? FolderLocation { get; set; }

        /// <summary>
        /// Gets or sets the template ID for template input type.
        /// </summary>
        [JsonPropertyName("template-id")]
        public string? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the legacy file name field for file input type.
        /// </summary>
        [JsonPropertyName("file-name")]
        public string? Filename { get; set; }
    }

    /// <summary>
    /// Represents a template configuration for a prompt.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the template ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the template name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the applicability match configuration for the template.
        /// </summary>
        [JsonPropertyName("applicability")]
        public TemplateMatch Match { get; set; } = new TemplateMatch();

        /// <summary>
        /// Gets or sets the folder containing the template files.
        /// </summary>
        [JsonPropertyName("template-folder")]
        public string TemplateFolder { get; set; }

        /// <summary>
        /// Gets or sets the default user prompt for the template.
        /// </summary>
        [JsonPropertyName("default-user-prompt")]
        public string DefaultUserPrompt { get; set; }

        /// <summary>
        /// Gets or sets the metadata associated with the template.
        /// </summary>
        [JsonPropertyName("metadata")]
        [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the list of rules for the template.
        /// </summary>
        [JsonPropertyName("rules")]
        public List<string> Rules { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of input files for the template.
        /// </summary>
        [JsonPropertyName("input-files")]
        public List<InputFile> InputFiles { get; set; } = new List<InputFile>();
    }

    /// <summary>
    /// Represents a keyword rule for template matching.
    /// </summary>
    public class KeywordRule
    {
        /// <summary>
        /// Gets or sets the keyword for matching.
        /// </summary>
        [JsonPropertyName("word")]
        public string Word { get; set; } = "";   // exact token match, case-insensitive (e.g., "add")

        /// <summary>
        /// Gets or sets the weight of the keyword.
        /// </summary>
        [JsonPropertyName("weight")]
        public double Weight { get; set; } = 1;
    }

    /// <summary>
    /// Represents the match configuration for a template, including keywords, negatives, and priority.
    /// </summary>
    public class TemplateMatch
    {
        /// <summary>
        /// Gets or sets the list of keyword rules for matching.
        /// </summary>
        [JsonPropertyName("key-words")]
        public List<KeywordRule> Keywords { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of negative keyword rules.
        /// </summary>
        [JsonPropertyName("negatives")]
        public List<KeywordRule> Negatives { get; set; } = new();

        /// <summary>
        /// Gets or sets the priority for template matching.
        /// Used only to break ties deterministically.
        /// </summary>
        [JsonPropertyName("priority")]
        public int Priority { get; set; } = 0;
    }

    /// <summary>
    /// Represents the configuration for an MCP server.
    /// </summary>
    public class McpServer
    {
        /// <summary>
        /// Gets or sets the name of the MCP server.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the transport type for the MCP server.
        /// </summary>
        [JsonPropertyName("transport")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public McpTransport Transport { get; set; } = McpTransport.Process;

        /// <summary>
        /// Gets or sets the command to start the MCP server (for process transport).
        /// </summary>
        [JsonPropertyName("command")]
        public string? Command { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the MCP server command.
        /// </summary>
        [JsonPropertyName("args")]
        public List<string>? Args { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the MCP server process.
        /// </summary>
        [JsonPropertyName("workingDirectory")]
        public string? WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the environment variables for the MCP server process.
        /// </summary>
        [JsonPropertyName("env")]
        public Dictionary<string, string>? Env { get; set; }

        /// <summary>
        /// Gets or sets the URL for the MCP server (for SSE transport).
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the headers for the MCP server (for SSE transport).
        /// </summary>
        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MCP server is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the group name for the MCP server.
        /// </summary>
        [JsonPropertyName("group")]
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets the description for the MCP server.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the list of tool names to filter for the MCP server.
        /// </summary>
        [JsonPropertyName("toolFilter")]
        public List<string>? ToolFilter { get; set; }
    }

    /// <summary>
    /// Specifies the transport type for an MCP server.
    /// </summary>
    public enum McpTransport
    {
        /// <summary>
        /// Process-based transport.
        /// </summary>
        Process,
        /// <summary>
        /// Server-sent events (SSE) transport.
        /// </summary>
        Sse
    }
}
