using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal abstract class VisualStudioProject : IVisualStudioProject
    {
        protected VisualStudioProject(IElement element)
        {

            Id = element.Id;
            Name = element.Name;
            Folder = element.ParentElement != null ? new Folder(element.ParentElement) : null;
            Stereotypes = element.Stereotypes;
            RelativeLocation = this.GetStereotypeProperty<string>("Project Settings", "Relative Location");
            Roles = element.ChildElements.Where(x => x.SpecializationType == "Role").Select(x => x.Name).ToList();
        }

        public string Id { get; }
        public string Name { get; }
        public abstract string ProjectTypeId { get; }
        public abstract string Type { get; }
        public IList<string> Roles { get; }
        public string RelativeLocation { get; }
        public IFolder Folder { get; }
        public IEnumerable<IStereotype> Stereotypes { get; }

        public IProjectConfig ToProjectConfig()
        {
            return new ProjectConfig(this);
        }

        public abstract string TargetFrameworkVersion();
    }
}