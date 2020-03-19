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

        public IList<IAssociationEndSettings> GetAssociationEndSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == AssociationEndSettings.SpecializationType)
                .Select(x => new AssociationEndSettings(x))
                .ToList<IAssociationEndSettings>();
            return models;
        }

        public IList<IAssociationSettings> GetAssociationSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == AssociationSettings.SpecializationType)
                .Select(x => new AssociationSettings(x))
                .ToList<IAssociationSettings>();
            return models;
        }

        public IList<IAttributeSettings> GetAttributeSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == AttributeSettings.SpecializationType)
                .Select(x => new AttributeSettings(x))
                .ToList<IAttributeSettings>();
            return models;
        }

        public IList<ICSharpTemplate> GetCSharpTemplates(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == CSharpTemplate.SpecializationType)
                .Select(x => new CSharpTemplate(x))
                .ToList<ICSharpTemplate>();
            return models;
        }

        public IList<IContextMenu> GetContextMenus(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ContextMenu.SpecializationType)
                .Select(x => new ContextMenu(x))
                .ToList<IContextMenu>();
            return models;
        }

        public IList<ICreationOption> GetCreationOptions(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == CreationOption.SpecializationType)
                .Select(x => new CreationOption(x))
                .ToList<ICreationOption>();
            return models;
        }

        public IList<IDecorator> GetDecorators(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == Decorator.SpecializationType)
                .Select(x => new Decorator(x))
                .ToList<IDecorator>();
            return models;
        }

        public IList<IDiagramSettings> GetDiagramSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == DiagramSettings.SpecializationType)
                .Select(x => new DiagramSettings(x))
                .ToList<IDiagramSettings>();
            return models;
        }

        public IList<IElementSettings> GetElementSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ElementSettings.SpecializationType)
                .Select(x => new ElementSettings(x))
                .ToList<IElementSettings>();
            return models;
        }

        public IList<IElementVisualSettings> GetElementVisualSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ElementVisualSettings.SpecializationType)
                .Select(x => new ElementVisualSettings(x))
                .ToList<IElementVisualSettings>();
            return models;
        }

        public IList<IFileTemplate> GetFileTemplates(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == FileTemplate.SpecializationType)
                .Select(x => new FileTemplate(x))
                .ToList<IFileTemplate>();
            return models;
        }

        public IList<IFolder> GetFolders(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == Folder.SpecializationType)
                .Select(x => new Folder(x))
                .ToList<IFolder>();
            return models;
        }

        public IList<ILiteralSettings> GetLiteralSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == LiteralSettings.SpecializationType)
                .Select(x => new LiteralSettings(x))
                .ToList<ILiteralSettings>();
            return models;
        }

        public IList<IMappingSettings> GetMappingSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == MappingSettings.SpecializationType)
                .Select(x => new MappingSettings(x))
                .ToList<IMappingSettings>();
            return models;
        }

        public IList<IModeler> GetModelers(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == Modeler.SpecializationType)
                .Select(x => new Modeler(x))
                .ToList<IModeler>();
            return models;
        }

        public IList<IModelerReference> GetModelerReferences(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ModelerReference.SpecializationType)
                .Select(x => new ModelerReference(x))
                .ToList<IModelerReference>();
            return models;
        }

        public IList<IModelersFolder> GetModelersFolders(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == ModelersFolder.SpecializationType)
                .Select(x => new ModelersFolder(x))
                .ToList<IModelersFolder>();
            return models;
        }

        public IList<IOperationSettings> GetOperationSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == OperationSettings.SpecializationType)
                .Select(x => new OperationSettings(x))
                .ToList<IOperationSettings>();
            return models;
        }

        public IList<IPackageSettings> GetPackageSettings(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == PackageSettings.SpecializationType)
                .Select(x => new PackageSettings(x))
                .ToList<IPackageSettings>();
            return models;
        }

        public IList<ITypeDefinition> GetTypeDefinitions(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.SpecializationType == TypeDefinition.SpecializationType)
                .Select(x => new TypeDefinition(x))
                .ToList<ITypeDefinition>();
            return models;
        }

    }
}