using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Intent.Modules.Common.Plugins;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;
using Newtonsoft.Json;

namespace Intent.Modules.Angular
{
    [Description("Angular NgxBootrap Installer")]
    public class AngularCliNgxBootstrapInstaller : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override int Order => 200;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                var project = CliCommand.GetWebCoreProject(application);
                if (project == null)
                {
                    Logging.Log.Failure("Could not find project to install Angular application.");
                    return;
                }
                var appLocation = Path.Join(project.Location, "ClientApp");
                if (!IsNgxBootrapInstalled(appLocation))
                {
                    Logging.Log.Info($"Installing Ngx-Bootstrap into Angular app at location [{appLocation}]");
                    CliCommand.Run(appLocation, $@"ng add ngx-bootstrap");
                    CliCommand.Run(appLocation, $@"npm i ngx-bootstrap@5.3.2"); // Ensure this version
                }
                else
                {
                    Logging.Log.Info("Ngx-Bootstrap app already installed. Skipping installation");
                }
            }
        }

        public override string Id => "Intent.Angular.CLIInstaller";

        public bool IsNgxBootrapInstalled(string appLocation)
        {
            if (!File.Exists($@"{appLocation}/package.json"))
            {
                return true;
            }
            using (var file = File.OpenText($@"{appLocation}/package.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                var packageFile = (dynamic)serializer.Deserialize(new JsonTextReader(file));
                return packageFile.dependencies["ngx-bootstrap"] != null;
            }
        }
    }
}
