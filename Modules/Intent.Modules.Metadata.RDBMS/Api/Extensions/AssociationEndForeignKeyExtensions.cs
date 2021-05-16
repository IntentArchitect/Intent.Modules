using System;
using Intent.Modelers.Domain.Api;

namespace Intent.Metadata.RDBMS.Api
{
    public static class AssociationEndForeignKeyExtensions
    {
        public static bool RequiresForeignKey(this AssociationEndModel associationEnd)
        {
            return IsManyToVariantsOfOne(associationEnd) || IsSelfReferencingZeroToOne(associationEnd);
        }

        private static bool IsManyToVariantsOfOne(AssociationEndModel associationEnd)
        {
            return (associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne)
                   && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many;
        }

        private static bool IsSelfReferencingZeroToOne(AssociationEndModel associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.ZeroToOne && associationEnd.Association.TargetEnd.Class == associationEnd.Association.SourceEnd.Class;
        }
    }

    public static class AssociationExtensions
    {
        public static RelationshipType GetRelationshipType(this AssociationModel association)
        {
            if ((association.SourceEnd.Multiplicity == Multiplicity.One || association.SourceEnd.Multiplicity == Multiplicity.ZeroToOne) && (association.TargetEnd.Multiplicity == Multiplicity.One || association.TargetEnd.Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.OneToOne;
            if ((association.SourceEnd.Multiplicity == Multiplicity.One || association.SourceEnd.Multiplicity == Multiplicity.ZeroToOne) && association.TargetEnd.Multiplicity == Multiplicity.Many)
                return RelationshipType.OneToMany;
            if (association.SourceEnd.Multiplicity == Multiplicity.Many && (association.TargetEnd.Multiplicity == Multiplicity.One || association.TargetEnd.Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.ManyToOne;
            if (association.SourceEnd.Multiplicity == Multiplicity.Many && association.TargetEnd.Multiplicity == Multiplicity.Many)
                return RelationshipType.ManyToMany;

            throw new Exception($"The relationship type from [{association.SourceEnd.Class.Name}] to [{association.TargetEnd.Class.Name}] could not be determined.");
        }

        public static bool IsOneToOne(this AssociationModel association)
        {
            return association.GetRelationshipType() == RelationshipType.OneToOne;
        }

        public static bool IsOneToMany(this AssociationModel association)
        {
            return association.GetRelationshipType() == RelationshipType.OneToMany;
        }

        public static bool IsManyToOne(this AssociationModel association)
        {
            return association.GetRelationshipType() == RelationshipType.ManyToOne;
        }

        public static bool IsManyToMany(this AssociationModel association)
        {
            return association.GetRelationshipType() == RelationshipType.ManyToMany;
        }
    }

    public enum RelationshipType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany,
    }
}