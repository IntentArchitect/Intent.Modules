using System;
using System.Diagnostics;
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
        private const string ServicesPackageSpecializationId = "df45eaf6-9202-4c25-8dd5-677e9ba1e906";

        public OnInstallMigration(IPersistenceLoader persistenceLoader)
        {

            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.Security";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
            if (!app.GetDesigners().Any(d => d.Id == ApiMetadataDesignerExtensions.ServicesDesignerId))
            {
                return;
            }

            var designer = app.GetDesigner(ApiMetadataDesignerExtensions.ServicesDesignerId);
            // should never be null. Extra check
            if (designer is null)
            {
                return;
            }

            var packages = designer.GetPackages();

            // only add to services 
            foreach (var package in packages.Where(p => p.SpecializationTypeId == ServicesPackageSpecializationId))
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