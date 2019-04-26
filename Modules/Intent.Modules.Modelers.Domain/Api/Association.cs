using System;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public class Association : IAssociation, IEquatable<IAssociation>
    {
        private readonly Metadata.Models.IAssociation _association;

        public Association(Metadata.Models.IAssociation association)
        {
            _association = association;
        }

        public string Id => _association.Id;
        public IAssociationEnd SourceEnd => new AssociationEnd(_association.SourceEnd, this);
        public IAssociationEnd TargetEnd => new AssociationEnd(_association.TargetEnd, this);
        public AssociationType AssociationType => (AssociationType)_association.AssociationType;
        public string Comment => _association.Comment;


        public bool Equals(IAssociation other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IAssociation)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}