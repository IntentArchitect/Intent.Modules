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

        public IList<AssociationSettings> GetAssociationSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == AssociationSettings.SpecializationType)
                .Select(x => new AssociationSettings(x))
                .ToList<AssociationSettings>();
            return models;
        }

        public IList<ContextMenu> GetContextMenus(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ContextMenu.SpecializationType)
                .Select(x => new ContextMenu(x))
                .ToList<ContextMenu>();
            return models;
        }

        public IList<CoreType> GetCoreTypes(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == CoreType.SpecializationType)
                .Select(x => new CoreType(x))
                .ToList<CoreType>();
            return models;
        }

        public IList<Decorator> GetDecorators(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == Decorator.SpecializationType)
                .Select(x => new Decorator(x))
                .ToList<Decorator>();
            return models;
        }

        public IList<DiagramSettings> GetDiagramSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DiagramSettings.SpecializationType)
                .Select(x => new DiagramSettings(x))
                .ToList<DiagramSettings>();
            return models;
        }

        public IList<ElementSettingsModel> GetElementSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ElementSettingsModel.SpecializationType)
                .Select(x => new ElementSettingsModel(x))
                .ToList<ElementSettingsModel>();
            return models;
        }

        public IList<FileTemplateModel> GetFileTemplates(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == FileTemplateModel.SpecializationType)
                .Select(x => new FileTemplateModel(x))
                .ToList<FileTemplateModel>();
            return models;
        }

        public IList<FolderModel> GetFolders(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == FolderModel.SpecializationType)
                .Select(x => new FolderModel(x))
                .ToList<FolderModel>();
            return models;
        }

        public IList<ModelerModel> GetModelers(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ModelerModel.SpecializationType)
                .Select(x => new ModelerModel(x))
                .ToList<ModelerModel>();
            return models;
        }

        public IList<ModelersFolder> GetModelersFolders(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ModelersFolder.SpecializationType)
                .Select(x => new ModelersFolder(x))
                .ToList<ModelersFolder>();
            return models;
        }

        public IList<PackageSettingsModel> GetPackageSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == PackageSettingsModel.SpecializationType)
                .Select(x => new PackageSettingsModel(x))
                .ToList<PackageSettingsModel>();
            return models;
        }

        public IList<TemplateRegistration> GetTemplateRegistrations(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == TemplateRegistration.SpecializationType)
                .Select(x => new TemplateRegistration(x))
                .ToList<TemplateRegistration>();
            return models;
        }

        public IList<TypeDefinition> GetTypeDefinitions(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == TypeDefinition.SpecializationType)
                .Select(x => new TypeDefinition(x))
                .ToList<TypeDefinition>();
            return models;
        }

    }
}