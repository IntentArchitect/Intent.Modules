using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal class Folder : IFolder
    {
        public const string SpecializationType = "Folder";
        public Folder(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            Id = element.Id;
            Name = element.Name;
            ParentFolder = element.ParentElement != null ? new Folder(element.ParentElement) : null;
            Stereotypes = element.Stereotypes;
        }

        public string Id { get; }
        public string Name { get; }
        public IFolder ParentFolder { get; }
        public IEnumerable<IStereotype> Stereotypes { get; }
    }
}