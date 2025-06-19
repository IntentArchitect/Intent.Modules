using System.Diagnostics;
using System.Linq;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Migrations
{
    public class Migration_02_02_00_Alpha_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_02_02_00_Alpha_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Modelers.Services.DomainInteractions";
        [IntentFully]
        public string ModuleVersion => "2.2.0-alpha.0";

        public void Up()
        {
            var servicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
            var servicePackageTypeId = "df45eaf6-9202-4c25-8dd5-677e9ba1e906";
            var servicesPackages = _persistenceLoader.LoadCurrentApplication().GetDesigner(servicesDesignerId).GetPackages()
                .Where(x => x.SpecializationTypeId == servicePackageTypeId)
                .ToList();

            foreach (var package in servicesPackages)
            {
                foreach (var serviceInvocation in package.Associations.Where(x => x.AssociationTypeId == "3e69085c-fa2f-44bd-93eb-41075fd472f8")) // Service Invocation
                {
                    foreach (var mapping in serviceInvocation.TargetEnd.Mappings)
                    {
                        if (mapping.TypeId == "df692ffe-5d0c-40ee-9362-a483d929a8ec") // Old Call Service Mapping
                        {
                            mapping.TypeId = "a4c4c5cc-76df-48ed-9d4e-c35caf44b567"; // New Invocation Mapping
                            mapping.Type = "Invocation Mapping";
                        }
                    }
                }
                package.Save();
            }

        }

        public void Down()
        {
            // Use version control for rollback.
        }
    }
}