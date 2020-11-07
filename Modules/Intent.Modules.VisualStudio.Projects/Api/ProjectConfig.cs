using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common;

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
        public string ParentId => null;

        public IEnumerable<string> SupportedFrameworks => _project.TargetFrameworkVersion()
            .Select(x => x.Trim())
            .ToArray();
        public IEnumerable<IOutputTargetRole> Roles => _project.Roles;
        public IEnumerable<IOutputTargetTemplate> Templates => _project.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
    }

    internal class FolderOutputTarget : IOutputTargetConfig
    {
        private readonly FolderModel _model;

        public FolderOutputTarget(FolderModel model)
        {
            _model = model;
            Metadata = new Dictionary<string, object>()
            {
                { "Namespace Provider", model.GetFolderOptions()?.NamespaceProvider() ?? true }
            };
        }
        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => _model.InternalElement.SpecializationType; // Folder
        public string Name => _model.Name;
        public string RelativeLocation => _model.Name;
        public string ParentId => _model.InternalElement.ParentId;

        public IEnumerable<string> SupportedFrameworks => new string[0];
        public IEnumerable<IOutputTargetRole> Roles => _model.Roles;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
    }
}