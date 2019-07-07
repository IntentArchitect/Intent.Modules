using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Entities.Keys
{
    internal static class AssociationEndHelperMethods
    {
        public static bool RequiresForeignKey(this IAssociationEnd associationEnd)
        {
            return IsManyToVariantsOfOne(associationEnd) || IsSelfReferencingZeroToOne(associationEnd);
        }

        private static bool IsManyToVariantsOfOne(IAssociationEnd associationEnd)
        {
            return (associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne)
                   && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many;
        }

        private static bool IsSelfReferencingZeroToOne(IAssociationEnd associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.ZeroToOne && associationEnd.Association.TargetEnd.Class == associationEnd.Association.SourceEnd.Class;
        }
    }
}