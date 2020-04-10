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
        public static IList<AssociationSettings> GetAssociationSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetAssociationSettings(application);
        }

        public static IList<ContextMenu> GetContextMenus(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetContextMenus(application);
        }

        public static IList<CoreType> GetCoreTypes(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetCoreTypes(application);
        }

        public static IList<Decorator> GetDecorators(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDecorators(application);
        }

        public static IList<DiagramSettings> GetDiagramSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetDiagramSettings(application);
        }

        public static IList<ElementSettings> GetElementSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetElementSettings(application);
        }

        public static IList<FileTemplate> GetFileTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFileTemplates(application);
        }

        public static IList<Folder> GetFolders(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolders(application);
        }

        public static IList<Modeler> GetModelers(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModelers(application);
        }

        public static IList<ModelersFolder> GetModelersFolders(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetModelersFolders(application);
        }

        public static IList<PackageSettings> GetPackageSettings(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetPackageSettings(application);
        }

        public static IList<TemplateRegistration> GetTemplateRegistrations(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTemplateRegistrations(application);
        }

        public static IList<TypeDefinition> GetTypeDefinitions(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetTypeDefinitions(application);
        }

    }
}