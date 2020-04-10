using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<AssociationSettingsModel> GetAssociationSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == AssociationSettingsModel.SpecializationType)
                .Select(x => new AssociationSettingsModel(x))
                .ToList<AssociationSettingsModel>();
            return models;
        }

        public IList<ContextMenuModel> GetContextMenuModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ContextMenuModel.SpecializationType)
                .Select(x => new ContextMenuModel(x))
                .ToList<ContextMenuModel>();
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

        public IList<DiagramSettingsModel> GetDiagramSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DiagramSettingsModel.SpecializationType)
                .Select(x => new DiagramSettingsModel(x))
                .ToList<DiagramSettingsModel>();
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

        public IList<ModelerModel> GetModelerModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ModelerModel.SpecializationType)
                .Select(x => new ModelerModel(x))
                .ToList<ModelerModel>();
            return models;
        }

        public IList<ModelersFolderModel> GetModelersFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ModelersFolderModel.SpecializationType)
                .Select(x => new ModelersFolderModel(x))
                .ToList<ModelersFolderModel>();
            return models;
        }

        public IList<PackageSettingsModel> GetPackageSettingsModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == PackageSettingsModel.SpecializationType)
                .Select(x => new PackageSettingsModel(x))
                .ToList<PackageSettingsModel>();
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

        public IList<TypeDefinitionModel> GetTypeDefinitionModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
                .Select(x => new TypeDefinitionModel(x))
                .ToList<TypeDefinitionModel>();
            return models;
        }

    }
}