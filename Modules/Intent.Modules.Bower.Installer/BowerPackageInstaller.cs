using Intent.Modules.Bower.Templates.BowerConfig;
using Intent.Modules.Common.Plugins;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System;
using Intent.Modules.Common;

namespace Intent.Modules.Bower.Installer
{
    [Description("Bower Package Installer")]
    public class BowerPackageInstaller : FactoryExtensionBase, IExecutionLifeCycle
    {

        public BowerPackageInstaller()
        {
            Order = 10;
        }


        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                Run(application);
            }
        }

        public override string Id
        {
            get
            {
                return "Intent.BowerPackageInstaller";
            }
        }

        public void Run(IApplication application)
        {
            var projects = application.Projects.Where(x => x.HasTemplateInstance(BowerConfigTemplate.Identifier));
            foreach (var project in projects)
            {
                Logging.Log.Info($"Installing Bower Packages [{ project.Name }]");

                CommandLineProcessor cmd = new CommandLineProcessor();

                var output = cmd.ExecuteCommand(Path.GetFullPath(project.ProjectLocation),
                        new[]
                        {
                            @"PATH=.\node_modules\.bin;C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Web Tools\External;%PATH%;C:\Program Files\Git\bin",
                            "bower install",
                        });
                Logging.Log.Info(output);
            }
            Logging.Log.Info($"Completed Installing Bower Packages");
        }
    }
}