using Intent.Modules.Bower.Templates.BowerConfig;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins;
using System.IO;
using System.Linq;

namespace Intent.Modules.Bower.Installer
{
    public class BowerPackageInstaller : ApplicationProcessorBase, IApplicationProcessor
    {

        public BowerPackageInstaller()
        {
            Order = 10;
            WhenToRun = ApplicationProcessorExecutionStep.AfterTemplateExecution;
        }

        public override string Id
        {
            get
            {
                return "Intent.BowerPackageInstaller";
            }
        }

        public override void Run(IApplication application)
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