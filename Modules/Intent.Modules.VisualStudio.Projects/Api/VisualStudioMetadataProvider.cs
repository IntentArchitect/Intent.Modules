using System.Collections.Generic;
using System.Linq;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public class VisualStudioMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public VisualStudioMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        
        public IEnumerable<IVisualStudioProject> GetClassLibraryDotNetCoreProjects(string applicationId)
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Visual Studio")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == ClassLibraryDotNetCoreProject.SpecializationType).ToList();
            return types.Select(x => new ClassLibraryDotNetCoreProject(x)).ToList();
        }

        public IEnumerable<IVisualStudioProject> GetClassLibraryDotNetFrameworkProjects(string applicationId)
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Visual Studio")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == ClassLibraryDotNetFrameworkProject.SpecializationType).ToList();
            return types.Select(x => new ClassLibraryDotNetFrameworkProject(x)).ToList();
        }

        public IEnumerable<IVisualStudioProject> GetWebApplicationDotNetFrameworkProjects(string applicationId)
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Visual Studio")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == WebApplicationDotNetFrameworkProject.SpecializationType).ToList();
            return types.Select(x => new WebApplicationDotNetFrameworkProject(x)).ToList();
        }

        public IEnumerable<IVisualStudioProject> GetWebApplicationDotNetCoreProjects(string applicationId)
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Visual Studio")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == WebApplicationDotNetCoreProject.SpecializationType).ToList();
            return types.Select(x => new WebApplicationDotNetCoreProject(x)).ToList();
        }

        public IEnumerable<IVisualStudioProject> GetWcfServiceApplicationDotNetFrameworkProjects(string applicationId)
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Visual Studio")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == WcfServiceApplicationDotNetFrameworkProject.SpecializationType).ToList();
            return types.Select(x => new WcfServiceApplicationDotNetFrameworkProject(x)).ToList();
        }

        public IEnumerable<IVisualStudioProject> GetConsoleAppDotNetFrameworkProjects(string applicationId)
        {
            var types = _metadataManager.GetMetadata<Metadata.Models.IElement>("Visual Studio")
                .Where(x => x.Application.Id == applicationId && x.SpecializationType == ConsoleAppDotNetFrameworkProject.SpecializationType).ToList();
            return types.Select(x => new ConsoleAppDotNetFrameworkProject(x)).ToList();
        }
    }
}