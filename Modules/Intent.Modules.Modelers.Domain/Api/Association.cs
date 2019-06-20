using System;
using System.Collections.Generic;

namespace Intent.Modelers.Domain.Api
{
    public class Association : IAssociation, IEquatable<IAssociation>
    {
        private readonly Metadata.Models.IAssociation _association;
        private readonly IDictionary<string, Class> _classCache;

        internal Association(Metadata.Models.IAssociation association, IDictionary<string, Class> classCache)
        {
            _association = association;
            _classCache = classCache;
            SourceEnd = new AssociationEnd(_association.SourceEnd, this, _classCache);
            TargetEnd = new AssociationEnd(_association.TargetEnd, this, _classCache);
        }

        public string Id => _association.Id;
        public IAssociationEnd SourceEnd { get; }
        public IAssociationEnd TargetEnd { get; }
        public AssociationType AssociationType
        {
            get
            {
                switch (_association.SpecializationType.ToLower())
                {
                    case "composition":
                        return AssociationType.Composition;
                    case "aggregation":
                        return AssociationType.Aggregation;
                    case "generalization":
                        return AssociationType.Generalization;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(AssociationType), $"Unable to convert association specialization type '{_association.SpecializationType}' to known association type.");
                }
            }
        }

        public string Comment => _association.Comment;

        public override string ToString()
        {
            return $"[{SourceEnd}] {(SourceEnd.Multiplicity == Multiplicity.Many ? "*" : SourceEnd.Multiplicity == Multiplicity.One ? "1" : "0..1")}" +
                   $" --> " +
                   $"{(TargetEnd.Multiplicity == Multiplicity.Many ? "*" : TargetEnd.Multiplicity == Multiplicity.One ? "1" : "0..1")} [{TargetEnd}]";
        }

        public static bool operator ==(Association lhs, Association rhs)
        {
            // Check for null.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles the case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Association lhs, Association rhs)
        {
            return !(lhs == rhs);
        }

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