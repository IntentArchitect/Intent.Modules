using System.Collections.Generic;
using System.Linq;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class VisualStudioMetadataManagerExtensions
    {
        public static IEnumerable<IVisualStudioProject> GetAllProjects(this IMetadataManager metadataManager, string applicationId)
        {
            var manager = new VisualStudioMetadataProvider(metadataManager);
            return new IVisualStudioProject[0]
                .Concat(manager.GetClassLibraryDotNetCoreProjects(applicationId))
                .Concat(manager.GetClassLibraryDotNetFrameworkProjects(applicationId))
                .Concat(manager.GetWebApplicationDotNetCoreProjects(applicationId))
                .Concat(manager.GetWebApplicationDotNetFrameworkProjects(applicationId))
                .Concat(manager.GetWcfServiceApplicationDotNetFrameworkProjects(applicationId))
                .Concat(manager.GetConsoleAppDotNetFrameworkProjects(applicationId))
                ;
        }

        public static IEnumerable<IVisualStudioProject> GetClassLibraryDotNetCoreProjects(this IMetadataManager metadataManager, string applicationId)
        {
            return new VisualStudioMetadataProvider(metadataManager).GetClassLibraryDotNetCoreProjects(applicationId);
        }

        public static IEnumerable<IVisualStudioProject> GetClassLibraryDotNetFrameworkProjects(this IMetadataManager metadataManager, string applicationId)
        {
            return new VisualStudioMetadataProvider(metadataManager).GetClassLibraryDotNetFrameworkProjects(applicationId);
        }

        public static IEnumerable<IVisualStudioProject> GetWebApplicationDotNetCoreProjects(this IMetadataManager metadataManager, string applicationId)
        {
            return new VisualStudioMetadataProvider(metadataManager).GetWebApplicationDotNetCoreProjects(applicationId);
        }

        public static IEnumerable<IVisualStudioProject> GetWebApplicationDotNetFrameworkProjects(this IMetadataManager metadataManager, string applicationId)
        {
            return new VisualStudioMetadataProvider(metadataManager).GetWebApplicationDotNetFrameworkProjects(applicationId);
        }

        public static IEnumerable<IVisualStudioProject> GetWcfServiceApplicationDotNetFrameworkProjects(this IMetadataManager metadataManager, string applicationId)
        {
            return new VisualStudioMetadataProvider(metadataManager).GetWcfServiceApplicationDotNetFrameworkProjects(applicationId);
        }

        public static IEnumerable<IVisualStudioProject> GetConsoleAppDotNetFrameworkProjects(this IMetadataManager metadataManager, string applicationId)
        {
            return new VisualStudioMetadataProvider(metadataManager).GetConsoleAppDotNetFrameworkProjects(applicationId);
        }
    }
}