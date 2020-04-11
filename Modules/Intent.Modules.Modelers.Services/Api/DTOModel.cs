using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    internal class DTOModel : IDTOModel
    {
        private readonly IElement _class;
        public DTOModel(IElement @class)
        {
            _class = @class;
            Folder = _class.ParentElement?.SpecializationType == Api.FolderModel.SpecializationType ? new FolderModel(_class.ParentElement) : null;
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _class.Name;
        public IEnumerable<string> GenericTypes => _class.GenericTypes.Select(x => x.Name);
        public bool IsMapped => _class.IsMapped;
        public IElementMapping MappedClass => _class.MappedElement;
        public IElementApplication Application => _class.Application;
        public IEnumerable<IAttribute> Fields => _class.Attributes;
        public string Comment => _class.Id;

        protected bool Equals(DTOModel other)
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
            return Equals((DTOModel) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}