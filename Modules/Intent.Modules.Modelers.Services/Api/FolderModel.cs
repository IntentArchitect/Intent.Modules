using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    internal class FolderModel : IFolder
    {
        public const string SpecializationType = "Folder";
        public FolderModel(IElement element)
        {
            if (element.SpecializationType != "Folder")
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            Id = element.Id;
            Name = element.Name;
            ParentFolder = element.ParentElement != null ? new FolderModel(element.ParentElement) : null;
            IsPackage = false;
            Stereotypes = element.Stereotypes;
        }

        public string Id { get; }
        public string Name { get; }
        public IFolder ParentFolder { get; }
        public bool IsPackage { get; }
        public IEnumerable<IStereotype> Stereotypes { get; }

        protected bool Equals(FolderModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FolderModel) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}