using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Tasks
{
    [IntentManaged(Mode.Merge)]
    public class IModSpecChecker : IModuleTask
    {
        private readonly ISolutionConfig _solution;
        private readonly IMetadataManager _metadataManager;
        private readonly JsonSerializerOptions _serializerOptions;

        [IntentManaged(Mode.Merge)]
        public IModSpecChecker(ISolutionConfig solution, IMetadataManager metadataManager)
        {
            _solution = solution;
            _metadataManager = metadataManager;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public string TaskTypeId => "Intent.ModuleBuilder.IModSpecChecker";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "I mod spec checker";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public int Order => 0;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Execute(params string[] args)
        {
            Logging.Log.Info($"Args: {string.Join(",", args)}");

            if (args.Length < 1)
                return Fail("Expected 1 argument, received 0.");

            TaskConfig config;
            try
            {
                config = JsonSerializer.Deserialize<TaskConfig>(args[0], _serializerOptions)
                    ?? throw new InvalidOperationException($"Unable to deserialize: {args[0]}");
            }
            catch (Exception e)
            {
                return Fail($"Failed to deserialize task arguments: {e.Message}");
            }

            try
            {
                var outputPath = ModuleOutputPathHelper.ResolveIModSpecDirectory(
                    _metadataManager, _solution, config.ApplicationId);

                if (outputPath == null)
                    return Fail($"Application not found for ID: {config.ApplicationId}");

                var imodspecFile = Directory.GetFiles(outputPath, "*.imodspec").FirstOrDefault();
                if (imodspecFile == null)
                    return Fail($"No .imodspec file found in: {outputPath}");

                var doc = XDocument.Load(imodspecFile);
                var root = doc.Root;

                var version = root?.Element("version")?.Value;
                var id = root?.Element("id")?.Value;

                if (string.IsNullOrWhiteSpace(version))
                    return Fail($"Could not read version from: {imodspecFile}");

                Logging.Log.Info($"IModSpec found: {id} v{version}");

                return JsonSerializer.Serialize(new { id, version }, _serializerOptions);
            }
            catch (Exception e)
            {
                Logging.Log.Failure(e);
                return Fail(e.GetBaseException().Message);
            }
        }

        private string Fail(string reason)
        {
            Logging.Log.Failure(reason);
            return JsonSerializer.Serialize(new { errorMessage = reason });
        }

        private class TaskConfig
        {
            public string ApplicationId { get; set; } = "";
        }
    }
}
