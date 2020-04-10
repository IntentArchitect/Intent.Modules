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
        public static IList<IAssociationSettings> GetAssociationSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAssociationSettings(application);
        }

        public static IList<IContextMenu> GetContextMenus(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetContextMenus(application);
        }

        public static IList<ICoreType> GetCoreTypes(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetCoreTypes(application);
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

        public static IList<IFileTemplate> GetFileTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFileTemplates(application);
        }

        public static IList<IFolder> GetFolders(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolders(application);
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