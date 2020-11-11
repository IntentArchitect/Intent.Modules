using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Plugins;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Processors;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Angular
{
    [Description("DotNet CLI - Build")]
    public class AutoCompileModuleExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override int Order => 100;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            { 
                RunAngularCli(application);
            }
        }

        public override string Id => "Intent.ModuleBuilder.AutoCompile";
        
        public void RunAngularCli(IApplication application)
        {
            var location = GetRootExecutionLocation(application);
            if (location == null)
            {
                Logging.Log.Failure("Could not find location to run dotnet build command.");
                return;
            }

            var cmd = new CommandLineProcessor();

            if (!Directory.Exists(Path.GetFullPath(location)))
            {
                Logging.Log.Warning($"Could not build module because the path was not found: " + Path.GetFullPath(location));
            }
            var command = $@"dotnet build";
            Logging.Log.Info($"Executing: \"{command}\" at location \"{ Path.GetFullPath(location) }\"");
            try
            {
                var output = cmd.ExecuteCommand(Path.GetFullPath(location),
                    new[]
                    {
                        command,
                    });
                Logging.Log.Info(output);
            }
            catch (Exception e)
            {
                Logging.Log.Failure($@"Failed to execute: ""{command}""
Auto-compiling of module failed. If the problem persists, consider disabling this extension. Please see reasons below:");
                Logging.Log.Failure(e);
            }
        }

        private string GetRootExecutionLocation(IApplication application)
        {
            return application.OutputTargets.FirstOrDefault(x => x.HasTemplateInstances(IModSpecTemplate.TemplateId) || x.IsVSProject())?.Location;
        }
    }
}
