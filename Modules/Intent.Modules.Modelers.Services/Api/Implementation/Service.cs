using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    internal class Service : IServiceModel
    {
        private readonly IElement _element;
        public Service(IElement element)
        {
            _element = element;
            Folder = _element.ParentElement?.SpecializationType == Api.Folder.SpecializationType ? new Folder(_element.ParentElement) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _element.Name;
        public string ApplicationName => _element.Application.Name;
        public IElementApplication Application => _element.Application;
        public IEnumerable<IOperation> Operations => _element.Operations;
        public string Comment => _element.Id;

        public override string ToString()
        {
            return $"Service: {Name}";
        }

        protected bool Equals(Service other)
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
            return Equals((Service) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}