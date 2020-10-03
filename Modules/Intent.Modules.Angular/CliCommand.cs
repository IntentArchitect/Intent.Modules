using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Bower.Installer;
using Intent.Modules.Common.Processors;
using Intent.Modules.Constants;
using Intent.Utils;

namespace Intent.Modules.Angular
{
    public static class CliCommand
    {
        public static void Run(string location, string command)
        {
            CommandLineProcessor cmd = new CommandLineProcessor();

            if (!Directory.Exists(Path.GetFullPath(location)))
            {
                Directory.CreateDirectory(Path.GetFullPath(location));
            }
            try
            {
                Logging.Log.Info($"Executing: {location}> {command}");
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
Please ensure that both npm and that the Angular CLI have been installed.
To check that you have the npm client installed, run npm -v in a terminal/console window.
To install the CLI using npm, open a terminal/console window and enter the following command: npm install -g @angular/cli.");
                Logging.Log.Failure(e);
            }

        }

        public static IOutputTarget GetWebCoreProject(IApplication application)
        {
            return application.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.CoreWebApp);
        }
    }
}