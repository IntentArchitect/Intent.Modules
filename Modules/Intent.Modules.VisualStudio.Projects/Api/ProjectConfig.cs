using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal class ProjectConfig : IProjectConfig
    {
        private readonly IVisualStudioProject _project;

        public ProjectConfig(IVisualStudioProject project)
        {
            _project = project;
        }

        public IEnumerable<IStereotype> Stereotypes => _project.Stereotypes;
        public string Id => _project.Id;
        public string Type => _project.ProjectTypeId;
        public string Name => _project.Name;
        public string RelativeLocation => _project.RelativeLocation ?? _project.Name;
        public string TargetFrameworks => _project.TargetFrameworkVersion();
        public IList<IProjectOutputTarget> Roles => _project.GetRoles();
    }
}