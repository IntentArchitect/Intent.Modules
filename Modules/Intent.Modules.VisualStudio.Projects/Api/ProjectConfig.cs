using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal class ProjectConfig : IOutputTargetConfig
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
        public IEnumerable<string> SupportedFrameworks => _project.TargetFrameworkVersion()
            .Select(x => x.Trim())
            .ToArray();
        public IList<IOutputTargetRole> Roles => _project.GetRoles();
    }
}