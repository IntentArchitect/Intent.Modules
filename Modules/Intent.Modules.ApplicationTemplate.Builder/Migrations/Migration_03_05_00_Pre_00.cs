using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Migrations
{
    public class Migration_03_05_00_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private const string AppTemplatesDesignerId = "22091d1e-a855-41af-ba7f-3f0b033c0fc3";
        private const string ModuleElementTypeId = "ef75f8f0-520c-4ab8-814f-5e75f4877dd7";
        private const string ModuleSettingsStereotypeId = "7033e6f4-2a22-4357-bd65-f0ec06c516d5";

        public Migration_03_05_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.ApplicationTemplate.Builder";
        [IntentFully]
        public string ModuleVersion => "3.5.0-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(AppTemplatesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(ModuleElementTypeId);
                if (elements.Count == 0)
                {
                    continue;
                }

                foreach (var element in elements)
                {
                    var settings = element.Stereotypes.FirstOrDefault(x => x.DefinitionId == ModuleSettingsStereotypeId);
                    if (settings == null)
                    {
                        continue;
                    }

                    var installMetadataOnly = GetOrCreateProperty(settings, "3f426089-6e92-49cf-a938-bda52e02b949", "Install Metadata Only");
                    var enableFactoryExtensions = GetOrCreateProperty(settings, "8a49ebad-ccb5-4860-aa4d-1dd6a4470ddc", "Enable Factory Extensions");
                    var installApplicationSettings = GetOrCreateProperty(settings, "e20913c9-8fd0-4802-b55c-754bd660c0db", "Install Application Settings");
                    var installDesignerMetadata = GetOrCreateProperty(settings, "42c467e4-4863-4bb4-a7b0-63a53cee1cb7", "Install Designer Metadata");
                    var installDesigners = GetOrCreateProperty(settings, "1c1e1606-2ad5-4924-8d0f-8ed33623c5bd", "Install Designers");
                    var installTemplateOutputs = GetOrCreateProperty(settings, "b1fdba70-0330-4dbe-9318-8bf756b5f506", "Install Template Outputs");

                    var targetValue = installMetadataOnly.Value == "true" ? "false" : "true";
                    enableFactoryExtensions.Value = targetValue;
                    installApplicationSettings.Value = targetValue;
                    installDesignerMetadata.Value = targetValue;
                    installDesigners.Value = targetValue;
                    installTemplateOutputs.Value = targetValue;

                    settings.Properties.Remove(installMetadataOnly);
                }

                package.Save(true);
            }
        }


        public void Down()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(AppTemplatesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(ModuleElementTypeId);
                if (elements.Count == 0)
                {
                    continue;
                }

                foreach (var element in elements)
                {
                    var settings = element.Stereotypes.FirstOrDefault(x => x.DefinitionId == ModuleSettingsStereotypeId);
                    if (settings == null)
                    {
                        continue;
                    }

                    var installMetadataOnly = GetOrCreateProperty(settings, "3f426089-6e92-49cf-a938-bda52e02b949", "Install Metadata Only");
                    var enableFactoryExtensions = GetOrCreateProperty(settings, "8a49ebad-ccb5-4860-aa4d-1dd6a4470ddc", "Enable Factory Extensions");
                    var installApplicationSettings = GetOrCreateProperty(settings, "e20913c9-8fd0-4802-b55c-754bd660c0db", "Install Application Settings");
                    var installDesignerMetadata = GetOrCreateProperty(settings, "42c467e4-4863-4bb4-a7b0-63a53cee1cb7", "Install Designer Metadata");
                    var installDesigners = GetOrCreateProperty(settings, "1c1e1606-2ad5-4924-8d0f-8ed33623c5bd", "Install Designers");
                    var installTemplateOutputs = GetOrCreateProperty(settings, "b1fdba70-0330-4dbe-9318-8bf756b5f506", "Install Template Outputs");

                    installMetadataOnly.Value = enableFactoryExtensions.Value != "true" ||
                                                installApplicationSettings.Value != "true" ||
                                                installDesignerMetadata.Value != "true" ||
                                                installDesigners.Value != "true" ||
                                                installTemplateOutputs.Value != "true"
                        ? "true"
                        : "false";

                    settings.Properties.Remove(enableFactoryExtensions);
                    settings.Properties.Remove(installApplicationSettings);
                    settings.Properties.Remove(installDesignerMetadata);
                    settings.Properties.Remove(installDesigners);
                    settings.Properties.Remove(installTemplateOutputs);
                }

                package.Save(true);
            }
        }

        private static StereotypePropertyPersistable GetOrCreateProperty(StereotypePersistable settings, string id, string name)
        {
            var property = settings.Properties.SingleOrDefault(x => x.Name == name);
            if (property == null)
            {
                property = new StereotypePropertyPersistable
                {
                    DefinitionId = id,
                    Name = name,
                    IsActive = true
                };
                settings.Properties.Add(property);
            }

            return property;
        }
    }
}