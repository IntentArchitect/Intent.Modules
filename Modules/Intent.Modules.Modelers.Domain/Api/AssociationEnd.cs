using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public class AssociationEnd : IAssociationEnd, IEquatable<IAssociationEnd>
    {
        private readonly Metadata.Models.IAssociationEnd _associationEnd;

        public AssociationEnd(Metadata.Models.IAssociationEnd associationEnd, IAssociation association)
        {
            _associationEnd = associationEnd;
            Association = association;
        }

        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;
        public IFolder Folder => _associationEnd.Folder;
        public string Id => _associationEnd.Id;
        public string Name => _associationEnd.Name;
        public string SpecializationType => _associationEnd.SpecializationType;
        public bool IsNullable => _associationEnd.IsNullable;
        public bool IsCollection => _associationEnd.IsCollection;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        public string Comment => _associationEnd.Comment;
        public IAssociation Association { get; }
        public IClass Class => new Class(_associationEnd.Class);
        public bool IsNavigable => _associationEnd.IsNavigable;
        public string MinMultiplicity => _associationEnd.MinMultiplicity;
        public string MaxMultiplicity => _associationEnd.MaxMultiplicity;
        public Multiplicity Multiplicity => (Multiplicity)_associationEnd.Multiplicity;
        public IAssociationEnd OtherEnd()
        {
            return Association.TargetEnd == this ? Association.SourceEnd : Association.TargetEnd;
        }


        public bool Equals(IAssociationEnd other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IAssociationEnd)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}