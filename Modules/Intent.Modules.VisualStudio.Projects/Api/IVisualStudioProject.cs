using System;
using System.CodeDom;
using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public interface IVisualStudioProject : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }
        string Type { get; }
        IList<string> Roles { get; }

        IProjectConfig ToProjectConfig();
    }

    internal abstract class VisualStudioProject : IVisualStudioProject
    {
        protected VisualStudioProject(IElement element)
        {

            Id = element.Id;
            Name = element.Name;
            Folder = element.ParentElement != null ? new Folder(element.ParentElement) : null;
            Stereotypes = element.Stereotypes;
            throw new NotImplementedException("Implement Roles");
        }

        public string Id { get; }
        public string Name { get; }
        public abstract string Type { get; }
        public IList<string> Roles { get; }
        public IFolder Folder { get; }
        public IEnumerable<IStereotype> Stereotypes { get; }

        public IProjectConfig ToProjectConfig()
        {
            return new ProjectConfig(this);
        }
    }

    internal class ProjectConfig : IProjectConfig
    {
        private readonly IVisualStudioProject _project;

        public ProjectConfig(IVisualStudioProject project)
        {
            _project = project;
        }

        public IEnumerable<IStereotype> Stereotypes => _project.Stereotypes;
        public string Id => _project.Id;
        public string Type => _project.Id;
        public string Name => _project.Id;
        public string RelativeLocation => _project.Id;
        public string TargetFrameworks => _project.Id;
        public IList<string> Roles => _project.Roles;
    }

    internal class ClassLibraryDotNetProject : VisualStudioProject
    {
        public const string SpecializationType = "ClassLibraryDotNetProject";
        public ClassLibraryDotNetProject(IElement element) : base(element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Cannot create a {nameof(ClassLibraryDotNetProject)} from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
        }

        public override string Type => SpecializationType;
    }
}