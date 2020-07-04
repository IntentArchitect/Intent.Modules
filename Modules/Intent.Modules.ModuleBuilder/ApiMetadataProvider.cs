using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }


        // Generated for each "Designer"
        public IEnumerable<IElement> GetModuleBuilderElements(IApplication application)
        {
            return _metadataManager.GetMetadata<IElement>("Module Builder", application.Id);
        }

        public IEnumerable<IAssociation> GetModuleBuilderAssociations(IApplication application)
        {
            return _metadataManager.GetMetadata<IAssociation>("Module Builder", application.Id);
        }

        public IEnumerable<IPackage> GetModuleBuilderPackages(IApplication application)
        {
            return _metadataManager.GetMetadata<IPackage>("Module Builder", application.Id);
        }

        public IList<AssociationSettingsModel> GetAssociationSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == AssociationSettingsModel.SpecializationType)
                .Select(x => new AssociationSettingsModel(x))
                .ToList<AssociationSettingsModel>();
            return models;
        }

        public IList<CoreTypeModel> GetCoreTypeModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == CoreTypeModel.SpecializationType)
                .Select(x => new CoreTypeModel(x))
                .ToList<CoreTypeModel>();
            return models;
        }

        public IList<DecoratorModel> GetDecoratorModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DecoratorModel.SpecializationType)
                .Select(x => new DecoratorModel(x))
                .ToList<DecoratorModel>();
            return models;
        }

        public IList<DesignerModel> GetDesignerModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DesignerModel.SpecializationType)
                .Select(x => new DesignerModel(x))
                .ToList<DesignerModel>();
            return models;
        }

        public IList<DesignerExtensionModel> GetDesignerExtensionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DesignerExtensionModel.SpecializationType)
                .Select(x => new DesignerExtensionModel(x))
                .ToList<DesignerExtensionModel>();
            return models;
        }

        public IList<DesignerSettingsModel> GetDesignerSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DesignerSettingsModel.SpecializationType)
                .Select(x => new DesignerSettingsModel(x))
                .ToList<DesignerSettingsModel>();
            return models;
        }

        public IList<DesignersFolderModel> GetDesignersFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DesignersFolderModel.SpecializationType)
                .Select(x => new DesignersFolderModel(x))
                .ToList<DesignersFolderModel>();
            return models;
        }

        public IList<DiagramSettingsModel> GetDiagramSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DiagramSettingsModel.SpecializationType)
                .Select(x => new DiagramSettingsModel(x))
                .ToList<DiagramSettingsModel>();
            return models;
        }

        public IList<ElementExtensionModel> GetElementExtensionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ElementExtensionModel.SpecializationType)
                .Select(x => new ElementExtensionModel(x))
                .ToList<ElementExtensionModel>();
            return models;
        }

        public IList<ElementSettingsModel> GetElementSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ElementSettingsModel.SpecializationType)
                .Select(x => new ElementSettingsModel(x))
                .ToList<ElementSettingsModel>();
            return models;
        }

        public IList<FileTemplateModel> GetFileTemplateModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == FileTemplateModel.SpecializationType)
                .Select(x => new FileTemplateModel(x))
                .ToList<FileTemplateModel>();
            return models;
        }

        public IList<FolderModel> GetFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == FolderModel.SpecializationType)
                .Select(x => new FolderModel(x))
                .ToList<FolderModel>();
            return models;
        }

        public IList<PackageExtensionModel> GetPackageExtensionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == PackageExtensionModel.SpecializationType)
                .Select(x => new PackageExtensionModel(x))
                .ToList<PackageExtensionModel>();
            return models;
        }

        public IList<ScriptModel> GetScriptModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ScriptModel.SpecializationType)
                .Select(x => new ScriptModel(x))
                .ToList<ScriptModel>();
            return models;
        }

        public IList<TemplateRegistrationModel> GetTemplateRegistrationModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == TemplateRegistrationModel.SpecializationType)
                .Select(x => new TemplateRegistrationModel(x))
                .ToList<TemplateRegistrationModel>();
            return models;
        }

    }
}