using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<IAssociationEndSettings> GetAssociationEndSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAssociationEndSettings(application);
        }

        public static IList<IAssociationSettings> GetAssociationSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAssociationSettings(application);
        }

        public static IList<IAttributeSettings> GetAttributeSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAttributeSettings(application);
        }

        public static IList<ICSharpTemplate> GetCSharpTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetCSharpTemplates(application);
        }

        public static IList<IContextMenu> GetContextMenus(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetContextMenus(application);
        }

        public static IList<ICoreType> GetCoreTypes(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetCoreTypes(application);
        }

        public static IList<ICreationOption> GetCreationOptions(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetCreationOptions(application);
        }

        public static IList<IDecorator> GetDecorators(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDecorators(application);
        }

        public static IList<IDiagramSettings> GetDiagramSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDiagramSettings(application);
        }

        public static IList<IElementSettings> GetElementSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetElementSettings(application);
        }

        public static IList<IElementVisualSettings> GetElementVisualSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetElementVisualSettings(application);
        }

        public static IList<IFileTemplate> GetFileTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFileTemplates(application);
        }

        public static IList<IFolder> GetFolders(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolders(application);
        }

        public static IList<ILiteralSettings> GetLiteralSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetLiteralSettings(application);
        }

        public static IList<IMappingSettings> GetMappingSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetMappingSettings(application);
        }

        public static IList<IModeler> GetModelers(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModelers(application);
        }

        public static IList<IModelerReference> GetModelerReferences(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModelerReferences(application);
        }

        public static IList<IModelersFolder> GetModelersFolders(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModelersFolders(application);
        }

        public static IList<IOperationSettings> GetOperationSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetOperationSettings(application);
        }

        public static IList<IPackageSettings> GetPackageSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetPackageSettings(application);
        }

        public static IList<ITemplateRegistration> GetTemplateRegistrations(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTemplateRegistrations(application);
        }

        public static IList<ITypeDefinition> GetTypeDefinitions(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTypeDefinitions(application);
        }

    }
}