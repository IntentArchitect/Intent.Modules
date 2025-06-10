using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.EventInteractions.Migrations
{
    public class Migration_2_0_0_alpha_0 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;

        public Migration_2_0_0_alpha_0(IPersistenceLoader persistenceLoader, IApplicationConfigurationProvider applicationConfigurationProvider)
        {
            _persistenceLoader = persistenceLoader;
            _applicationConfigurationProvider = applicationConfigurationProvider;
        }

        public string ModuleId { get; } = "Intent.Modelers.Services.EventInteractions";
        public string ModuleVersion { get; } = "2.0.0-pre.0";
        public const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";

        public void Up()
        {
            Debugger.Launch();
            var application = _persistenceLoader.LoadApplication(_applicationConfigurationProvider.GetApplicationConfig().FilePath);
            var designer = application.GetDesigner(ServicesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var deprecatedSendCommandAssociations = package.Associations.Where(x =>
                    x.AssociationTypeId == "38a3de5a-ca88-4f6e-88b9-88e5953936b2" // Send Command
                    || x.AssociationTypeId == "9510ff76-fba3-4eca-a5dd-0cfefc8f5bb6" // Call Service Operation
                    );
                if (!deprecatedSendCommandAssociations.Any())
                {
                    continue;
                }

                foreach (var association in deprecatedSendCommandAssociations)
                {
                    association.AssociationType = "Service Invocation";
                    association.AssociationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
                    association.SourceEnd.SpecializationType = "Service Invocation Source End";
                    association.SourceEnd.SpecializationTypeId = "ee56bd48-8eff-4fff-8d3a-87731d002335";
                    association.TargetEnd.SpecializationType = "Service Invocation Target End";
                    association.TargetEnd.SpecializationTypeId = "093e5909-ffe4-4510-b3ea-532f30212f3c";
                    foreach (var mapping in association.TargetEnd.Mappings.Where(x =>
                                     x.TypeId == "a4c4c5cc-76df-48ed-9d4e-c35caf44b567" // Integration Event Handler Mapping
                             ))
                    {
                        mapping.Type = "Invocation Mapping";
                        mapping.TypeId = "a4c4c5cc-76df-48ed-9d4e-c35caf44b567";
                    }
                }

                package.Save();
            }
        }

        public void Down()
        {
        }
    }
}