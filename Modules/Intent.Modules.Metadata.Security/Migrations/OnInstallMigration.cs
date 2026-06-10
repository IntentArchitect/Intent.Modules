using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Security.Api;
using Intent.Modelers.Services.Api;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Metadata.Security.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IPersistenceLoader persistenceLoader)
        {

            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.Security";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
            var designer = app.TryGetDesigner(ApiMetadataDesignerExtensions.ServicesDesignerId);
            if (designer is null)
            {
                return;
            }

            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                if (!package.GetElementsOfType(SecurityConfigurationModel.SpecializationTypeId).Any())
                {
                    package.Classes.Add(
                        id: Guid.NewGuid().ToString(),
                        specializationType: SecurityConfigurationModel.SpecializationType,
                        specializationTypeId: SecurityConfigurationModel.SpecializationTypeId,
                        name: "Security",
                        parentId: package.Id
                        );
                    package.Save();
                }
            }

        }
    }
}