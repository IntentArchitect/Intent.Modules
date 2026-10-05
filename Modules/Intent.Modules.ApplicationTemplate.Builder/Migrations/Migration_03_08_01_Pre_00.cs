using System.Collections.Generic;
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
    public class Migration_03_08_01_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private const string AppTemplatesDesignerId = "22091d1e-a855-41af-ba7f-3f0b033c0fc3";
        private const string DefaultsStereotypeId = "e99df2da-e5dd-41c0-bd03-c642cec089a8";

        private const string MetadataInSubfolderId = "48469188-f2bb-43e6-a83c-349d90fb7433";
        private const string MetadataInSubfolderName = "Metadata in Subfolder";
        private const string PlaceInSameDirectoryName = "Place solution and application in the same directory";

        private const string CreateFolderForSolutionId = "5c7741b1-0d43-4c93-a254-45252a87fa70";
        private const string CreateFolderForSolutionName = "Create folder for solution";

        private const string SeparateIntentFilesId = "54abe4ba-f663-4f73-8072-5c2da7ebbcd7";
        private const string SeparateIntentFilesName = "Store Intent Architect files separate to codebase";

        public Migration_03_08_01_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.ApplicationTemplate.Builder";
        [IntentFully]
        public string ModuleVersion => "3.8.1-pre.0";

        public void Up()
        {
            foreach (var (package, defaults) in GetApplicationTemplateDefaults())
            {
                // "Metadata in Subfolder" is the inverse of the old "Place solution and application in
                // the same directory", so invert the stored value to preserve existing behaviour.
                var metadataInSubfolder = GetOrCreateProperty(defaults, MetadataInSubfolderId, MetadataInSubfolderName);
                metadataInSubfolder.Value = Invert(metadataInSubfolder.Value);

                RemoveProperty(defaults, CreateFolderForSolutionId);
                RemoveProperty(defaults, SeparateIntentFilesId);

                package.Save(true);
            }
        }

        public void Down()
        {
            foreach (var (package, defaults) in GetApplicationTemplateDefaults())
            {
                var placeInSameDirectory = GetOrCreateProperty(defaults, MetadataInSubfolderId, PlaceInSameDirectoryName);
                placeInSameDirectory.Value = Invert(placeInSameDirectory.Value);

                // Both properties defaulted to "true" prior to their removal.
                GetOrCreateProperty(defaults, CreateFolderForSolutionId, CreateFolderForSolutionName).Value ??= "true";
                GetOrCreateProperty(defaults, SeparateIntentFilesId, SeparateIntentFilesName).Value ??= "true";

                package.Save(true);
            }
        }

        private IEnumerable<(PackageModelPersistable Package, StereotypePersistable Defaults)> GetApplicationTemplateDefaults()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.TryGetDesigner(AppTemplatesDesignerId);
            if (designer == null)
            {
                yield break;
            }

            foreach (var package in designer.GetPackages())
            {
                var defaults = package.Stereotypes.FirstOrDefault(x => x.DefinitionId == DefaultsStereotypeId);
                if (defaults == null)
                {
                    continue;
                }

                yield return (package, defaults);
            }
        }

        private static string Invert(string value)
        {
            // An absent or empty value means the property was never explicitly set, in which case the
            // definition's default of "false" applied.
            return value == "true" ? "false" : "true";
        }

        private static StereotypePropertyPersistable GetOrCreateProperty(StereotypePersistable stereotype, string definitionId, string name)
        {
            var property = stereotype.Properties.SingleOrDefault(x => x.DefinitionId == definitionId);
            if (property == null)
            {
                property = new StereotypePropertyPersistable
                {
                    DefinitionId = definitionId,
                    IsActive = true
                };
                stereotype.Properties.Add(property);
            }

            // The display name is persisted alongside the value, so it has to be kept in step with
            // whatever the stereotype definition currently calls the property.
            property.Name = name;

            return property;
        }

        private static void RemoveProperty(StereotypePersistable stereotype, string definitionId)
        {
            var property = stereotype.Properties.SingleOrDefault(x => x.DefinitionId == definitionId);
            if (property != null)
            {
                stereotype.Properties.Remove(property);
            }
        }
    }
}
