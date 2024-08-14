using Intent.Engine;
using Intent.Plugins;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Intent.Modules.ModuleBuilder.CSharp.Tasks
{
    public class NugetUpdaterTask : IModuleTask
    {
        private readonly IMetadataManager _metadataManager;
        private readonly JsonSerializerOptions _serializerOptions;

        public NugetUpdaterTask(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

        }

        public string TaskTypeId => "Intent.ModuleBuilder.CSharp.Tasks.NugetUpdater";

        public string TaskTypeName => "NuGet Packages Fetch Latest";

        public int Order => 0;

        public string Execute(params string[] args)
        {

            Logging.Log.Info($"Args: {string.Join(",", args)}");

            if (!ValiadateRequest(args, out var taskConfig, out var errorMessage))
            {
                return Fail(errorMessage!);
            }


            Logging.Log.Info($"Executing: {TaskTypeName}");
            try
            {
                var response = new TaskResponse();
                foreach (var package in taskConfig.NugetPackageIds)
                {
                    var result = NuGetApi.GetLatestVersionsForFrameworksAsync(package).GetAwaiter().GetResult();
                    response.NugetPackages.Add(new NugutPackageInfo(package, result));
                }
                return JsonSerializer.Serialize(response, _serializerOptions);
            }
            catch (Exception e)
            {
                Logging.Log.Failure(e);
                return Fail(e.GetBaseException().Message);
            }
        }

        private bool ValiadateRequest(string[] args, out TaskConfig? outSettings, out string? errorMessage)
        {
            outSettings = null;
            errorMessage = null;

            if (args.Length < 1)
            {
                errorMessage = $"Expected 1 argument received 0";
                return false;
            }
            var settings = JsonSerializer.Deserialize<TaskConfig>(args[0], _serializerOptions);
            if (settings == null)
            {
                errorMessage = $"Unable to deserialize : {args[0]}";
                return false;
            }

            outSettings = settings;

            return true;
        }

        private string Fail(string reason)
        {
            Logging.Log.Failure(reason);
            var errorObject = new { errorMessage = reason };
            string json = JsonSerializer.Serialize(errorObject);
            return json;
        }

    }
}
