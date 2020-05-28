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
    [IntentManaged(Mode.Merge)]
    public static class ApiMetadataProviderExtensions
    {
        [IntentManaged(Mode.Ignore)]
        public static IList<IVisualStudioProject> GetAllProjectModels(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.GetASPNETCoreWebApplicationModels(application).Cast<IVisualStudioProject>()
                .Concat(metadataManager.GetASPNETWebApplicationNETFrameworkModels(application))
                .Concat(metadataManager.GetClassLibraryNETCoreModels(application))
                .Concat(metadataManager.GetClassLibraryNETFrameworkModels(application))
                .Concat(metadataManager.GetConsoleAppNETFrameworkModels(application))
                .Concat(metadataManager.GetWCFServiceApplicationModels(application))
                .ToList();
        }


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

        public static IList<WCFServiceApplicationModel> GetWCFServiceApplicationModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetWCFServiceApplicationModels(application);
        }

        public static IList<SolutionFolderModel> GetSolutionFolderModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetSolutionFolderModels(application);
        }

    }
}