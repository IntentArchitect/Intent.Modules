using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<AssociationSettingsModel> GetAssociationSettingsModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAssociationSettingsModels(application);
        }

        public static IList<CoreTypeModel> GetCoreTypeModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetCoreTypeModels(application);
        }

        public static IList<DecoratorModel> GetDecoratorModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDecoratorModels(application);
        }

        public static IList<DesignerModel> GetDesignerModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDesignerModels(application);
        }

        public static IDesigner ModuleBuilder(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.GetDesigner(application.Id, "Module Builder");
        }

        public static IList<DesignerModel> GetDesignerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DesignerModel.SpecializationType)
                .Select(x => new DesignerModel(x))
                .ToList();
        }

        // _metadataManager.ModuleBuilder(application).GetDesignerModels();
        // _metadataManager.Domain(application).GetClassModels();

        public static IList<DesignerExtensionModel> GetDesignerExtensionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDesignerExtensionModels(application);
        }

        public static IList<DesignerSettingsModel> GetDesignerSettingsModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDesignerSettingsModels(application);
        }

        public static IList<DesignersFolderModel> GetDesignersFolderModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDesignersFolderModels(application);
        }

        public static IList<DiagramSettingsModel> GetDiagramSettingsModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDiagramSettingsModels(application);
        }

        public static IList<ElementExtensionModel> GetElementExtensionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetElementExtensionModels(application);
        }

        public static IList<ElementSettingsModel> GetElementSettingsModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetElementSettingsModels(application);
        }

        public static IList<FileTemplateModel> GetFileTemplateModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFileTemplateModels(application);
        }

        public static IList<FolderModel> GetFolderModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolderModels(application);
        }

        public static IList<PackageExtensionModel> GetPackageExtensionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetPackageExtensionModels(application);
        }

        public static IList<ScriptModel> GetScriptModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetScriptModels(application);
        }

        public static IList<TemplateRegistrationModel> GetTemplateRegistrationModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTemplateRegistrationModels(application);
        }

    }
}