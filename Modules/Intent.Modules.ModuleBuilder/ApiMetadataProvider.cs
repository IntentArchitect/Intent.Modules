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

        public IList<ElementSettings> GetElementSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ElementSettings.SpecializationType)
                .Select(x => new ElementSettings(x))
                .ToList<ElementSettings>();
            return models;
        }

        public IList<FileTemplate> GetFileTemplates(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == FileTemplate.SpecializationType)
                .Select(x => new FileTemplate(x))
                .ToList<FileTemplate>();
            return models;
        }

        public IList<Folder> GetFolders(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == Folder.SpecializationType)
                .Select(x => new Folder(x))
                .ToList<Folder>();
            return models;
        }

        public IList<Modeler> GetModelers(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == Modeler.SpecializationType)
                .Select(x => new Modeler(x))
                .ToList<Modeler>();
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

        public IList<PackageSettings> GetPackageSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == PackageSettings.SpecializationType)
                .Select(x => new PackageSettings(x))
                .ToList<PackageSettings>();
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