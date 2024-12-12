using System.Diagnostics;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using Path = Intent.IArchitect.CrossPlatform.IO.Path;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.Eventing.Migrations
{
    public class Migration_06_00_00_Pre_02 : IModuleMigration
    {
        private const string EventingDesignerId = "822e4254-9ced-4dd1-ad56-500b861f7e4d";
        private const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_06_00_00_Pre_02(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Modelers.Eventing";
        [IntentFully]
        public string ModuleVersion => "6.0.0-pre.2";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            // Because the eventing designer has already been uninstalled we need to manually load up its designer:
            var eventingDesignerPath = Path.Join(Path.GetDirectoryName(app.AbsolutePath), "Intent.Metadata/Eventing/Eventing.designer.config");
            Logging.Log.Info($"Checking for eventing designer at path {eventingDesignerPath}");

            if (!File.Exists(eventingDesignerPath))
            {
                Logging.Log.Info($"No file found at path");
                return;
            }

            var eventingDesigner = ApplicationDesignerPersistable.Load(eventingDesignerPath);
            var eventingPackages = eventingDesigner.GetPackages();

            if (eventingPackages.Count == 0)
            {
                Logging.Log.Info($"No eventing packages detected");
                return;
            }

            var servicesDesigner = app.GetDesigners()
                .Single(x => x.Id == ServicesDesignerId);

            var hasChange = false;
            foreach (var eventingPackage in eventingPackages)
            {
                Logging.Log.Info($"Processing eventing package at absolute path {eventingPackage.AbsolutePath}");
                if (servicesDesigner.PackageReferences.Any(x => x.AbsolutePath == eventingPackage.AbsolutePath))
                {
                    Logging.Log.Info($"Skipping as package already referenced in Services designer");
                    continue;
                }

                Logging.Log.Info($"Adding package reference to Services designer");
                hasChange = true;
                servicesDesigner.AddPackageReference(eventingPackage);
            }

            if (!hasChange)
            {
                Logging.Log.Info($"Finished processing with no package references added");
                return;
            }

            Logging.Log.Info($"Saving Services designer");
            servicesDesigner.Save();
        }

        public void Down()
        {
        }
    }
}