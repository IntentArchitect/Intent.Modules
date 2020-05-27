using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ASPNETCoreWebApplicationModel> GetASPNETCoreWebApplicationModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetASPNETCoreWebApplicationModels(application);
        }

        public static IList<ASPNETWebApplicationNETFrameworkModel> GetASPNETWebApplicationNETFrameworkModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetASPNETWebApplicationNETFrameworkModels(application);
        }

        public static IList<ClassLibraryNETCoreModel> GetClassLibraryNETCoreModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetClassLibraryNETCoreModels(application);
        }

        public static IList<ClassLibraryNETFrameworkModel> GetClassLibraryNETFrameworkModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetClassLibraryNETFrameworkModels(application);
        }

        public static IList<ConsoleAppNETFrameworkModel> GetConsoleAppNETFrameworkModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetConsoleAppNETFrameworkModels(application);
        }

        public static IList<FolderModel> GetFolderModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFolderModels(application);
        }

        public static IList<RoleModel> GetRoleModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetRoleModels(application);
        }

    }
}