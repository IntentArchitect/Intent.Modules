using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.SdkEvolutionHelpers;

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

        [Obsolete("Foreign keys are no longer managed via the associations. Use explicit Foreign Keys in your model.")]
        public static ForeignKey GetForeignKey(this AssociationEndModel model)
        {
            var stereotype = model.GetStereotype("Foreign Key");
            return stereotype != null ? new ForeignKey(stereotype) : null;
        }

        [Obsolete("Foreign keys are no longer managed via the associations. Use explicit Foreign Keys in your model.")]
        public static bool HasForeignKey(this AssociationEndModel model)
        {
            return model.HasStereotype("Foreign Key");
        }

        [Obsolete("Unique constraint stereotypes are replaced by unique indexes")]
        public static UniqueConstraint GetUniqueConstraint(this AssociationEndModel model)
        {
            var stereotype = model.GetStereotype("Unique Constraint");
            return stereotype != null ? new UniqueConstraint(stereotype) : null;
        }

        [Obsolete("Unique constraint stereotypes are replaced by unique indexes")]
        public static bool HasUniqueConstraint(this AssociationEndModel model)
        {
            return model.HasStereotype("Unique Constraint");
        }

        public class ForeignKey
        {
            private IStereotype _stereotype;

            public ForeignKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ColumnName()
            {
                return _stereotype.GetProperty<string>("Column Name");
            }

        }

        public class UniqueConstraint
        {
            private IStereotype _stereotype;

            public UniqueConstraint(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

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