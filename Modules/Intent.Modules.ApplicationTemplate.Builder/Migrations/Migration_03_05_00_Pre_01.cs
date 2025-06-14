using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Migrations
{
    public class Migration_03_05_00_Pre_01 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private const string AppTemplatesDesignerId = "22091d1e-a855-41af-ba7f-3f0b033c0fc3";
        private const string ModuleElementTypeId = "ef75f8f0-520c-4ab8-814f-5e75f4877dd7";
        private const string ModuleSettingsStereotypeId = "7033e6f4-2a22-4357-bd65-f0ec06c516d5";

        private class AssetType
        {
            public const string ApplicationSettings = "Application Settings";
            public const string DesignerMetadata = "Designer Metadata";
            public const string Designers = "Designers";
            public const string FactoryExtensions = "Factory Extensions";
            public const string TemplateOutputs = "Template Outputs";
        }

        public Migration_03_05_00_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.ApplicationTemplate.Builder";
        [IntentFully]
        public string ModuleVersion => "3.5.0-pre.1";

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

                    var includeAssets = GetOrCreateProperty(settings, "4bdd0a51-5a47-4119-b0bb-cd1240ac7850", "Include Assets");
                    var includedAssets = GetOrCreateProperty(settings, "478326e2-5290-428e-aef9-10f2c67f24c2", "Included Assets");

                    var enableFactoryExtensions = GetOrCreateProperty(settings, "8a49ebad-ccb5-4860-aa4d-1dd6a4470ddc", "Enable Factory Extensions");
                    var installApplicationSettings = GetOrCreateProperty(settings, "e20913c9-8fd0-4802-b55c-754bd660c0db", "Install Application Settings");
                    var installDesignerMetadata = GetOrCreateProperty(settings, "42c467e4-4863-4bb4-a7b0-63a53cee1cb7", "Install Designer Metadata");
                    var installDesigners = GetOrCreateProperty(settings, "1c1e1606-2ad5-4924-8d0f-8ed33623c5bd", "Install Designers");
                    var installTemplateOutputs = GetOrCreateProperty(settings, "b1fdba70-0330-4dbe-9318-8bf756b5f506", "Install Template Outputs");

                    var includedAssetsList = new List<string>(5);

                    if (enableFactoryExtensions.Value == "true") includedAssetsList.Add(AssetType.FactoryExtensions);
                    if (installApplicationSettings.Value == "true") includedAssetsList.Add(AssetType.ApplicationSettings);
                    if (installDesignerMetadata.Value == "true") includedAssetsList.Add(AssetType.DesignerMetadata);
                    if (installDesigners.Value == "true") includedAssetsList.Add(AssetType.Designers);
                    if (installTemplateOutputs.Value == "true") includedAssetsList.Add(AssetType.TemplateOutputs);

                    includeAssets.Value = includedAssetsList.Count switch
                    {
                        0 => "None",
                        5 => "All",
                        _ => "Select"
                    };
                    includedAssets.Value = includeAssets.Value == "Select"
                        ? JsonSerializer.Serialize(includedAssetsList)
                        : "[]";

                    settings.Properties.Remove(enableFactoryExtensions);
                    settings.Properties.Remove(installApplicationSettings);
                    settings.Properties.Remove(installDesignerMetadata);
                    settings.Properties.Remove(installDesigners);
                    settings.Properties.Remove(installTemplateOutputs);
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

                    var includeAssets = GetOrCreateProperty(settings, "4bdd0a51-5a47-4119-b0bb-cd1240ac7850", "Include Assets");
                    var includedAssets = GetOrCreateProperty(settings, "478326e2-5290-428e-aef9-10f2c67f24c2", "Included Assets");

                    var enableFactoryExtensions = GetOrCreateProperty(settings, "8a49ebad-ccb5-4860-aa4d-1dd6a4470ddc", "Enable Factory Extensions");
                    var installApplicationSettings = GetOrCreateProperty(settings, "e20913c9-8fd0-4802-b55c-754bd660c0db", "Install Application Settings");
                    var installDesignerMetadata = GetOrCreateProperty(settings, "42c467e4-4863-4bb4-a7b0-63a53cee1cb7", "Install Designer Metadata");
                    var installDesigners = GetOrCreateProperty(settings, "1c1e1606-2ad5-4924-8d0f-8ed33623c5bd", "Install Designers");
                    var installTemplateOutputs = GetOrCreateProperty(settings, "b1fdba70-0330-4dbe-9318-8bf756b5f506", "Install Template Outputs");

                    var includedAssetsList = !string.IsNullOrWhiteSpace(includedAssets.Value)
                        ? JsonSerializer.Deserialize<List<string>>(includedAssets.Value)
                        : [];

                    enableFactoryExtensions.Value = GetValue(includeAssets, includedAssetsList, AssetType.FactoryExtensions);
                    installApplicationSettings.Value = GetValue(includeAssets, includedAssetsList, AssetType.ApplicationSettings);
                    installDesignerMetadata.Value = GetValue(includeAssets, includedAssetsList, AssetType.DesignerMetadata);
                    installDesigners.Value = GetValue(includeAssets, includedAssetsList, AssetType.Designers);
                    installTemplateOutputs.Value = GetValue(includeAssets, includedAssetsList, AssetType.TemplateOutputs);

                    settings.Properties.Remove(includeAssets);
                    settings.Properties.Remove(includedAssets);
                }

                package.Save(true);
            }

            static string GetValue(StereotypePropertyPersistable includeAssets, List<string> includedAssets, string assetType)
            {
                return includeAssets.Value switch
                {
                    "All" => "true",
                    "None" => "false",
                    "Select" => includedAssets.Contains(assetType) ? "true" : "false",
                    _ => throw new InvalidOperationException($"Unknown value: {includeAssets.Value}")
                };
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