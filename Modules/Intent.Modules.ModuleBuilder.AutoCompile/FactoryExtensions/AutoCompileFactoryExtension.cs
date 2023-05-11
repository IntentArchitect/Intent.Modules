using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AutoCompile.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    [Description("DotNet CLI - Build")]
    public class AutoCompileFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.ModuleBuilder.AutoCompile.AutoCompileFactoryExtension";
        public override int Order => 100;

        protected override void OnAfterCommitChanges(IApplication application)
        {
            var location = GetRootExecutionLocation(application);
            if (location == null)
            {
                Logging.Log.Failure("Could not find location to run dotnet build command.");
                return;
            }

            if (!Directory.Exists(Path.GetFullPath(location)))
            {
                Logging.Log.Warning("Could not build module because the path was not found: " + Path.GetFullPath(location));
            }

            Logging.Log.Info($"Executing: \"dotnet build\" at location \"{Path.GetFullPath(location)}\"");
            try
            {
                var cmd = Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "build",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = location
                })!;

                cmd.WaitForExit();

                var output = cmd.StandardOutput.ReadToEnd();
                if (cmd.ExitCode == 0)
                {
                    Logging.Log.Info(output);
                }
                else
                {
                    Logging.Log.Failure(output);
                }
            }
            catch (Exception e)
            {
                Logging.Log.Failure(@"Failed to execute: ""dotnet build""
Auto-compiling of module failed. If the problem persists, consider disabling this extension. Please see reasons below:");
                Logging.Log.Failure(e);
            }
        }

        private static string GetRootExecutionLocation(IApplication application)
        {
            return application.OutputTargets.FirstOrDefault(x => x.HasTemplateInstances(IModSpecTemplate.TemplateId) || x.IsVSProject())?.Location;
        }
    }
}