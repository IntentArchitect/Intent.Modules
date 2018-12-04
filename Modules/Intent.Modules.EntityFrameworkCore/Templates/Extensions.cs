using System;
using Intent.MetaModel.Domain;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.EntityFrameworkCore.Templates
{
    public static class Extensions
    {
        public static string Name(this IAssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Name))
            {
                var className = associationEnd.Class.Name;
                if (associationEnd.MaxMultiplicity == "*" || int.Parse(associationEnd.MaxMultiplicity) > 1)
                {
                    return className.EndsWith("y") ? className.Substring(0, className.Length - 1) + "ies" : string.Format("{0}s", className);
                }
                return associationEnd.Class.Name;
            }

            return associationEnd.Name;
        }

        public static RelationshipType Relationship(this IAssociationEnd associationEnd)
        {
            if ((associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne) && (associationEnd.OtherEnd().Multiplicity == Multiplicity.One || associationEnd.OtherEnd().Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.OneToOne;
            if ((associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne) && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
                return RelationshipType.OneToMany;
            if (associationEnd.Multiplicity == Multiplicity.Many && (associationEnd.OtherEnd().Multiplicity == Multiplicity.One || associationEnd.OtherEnd().Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.ManyToOne;
            if (associationEnd.Multiplicity == Multiplicity.Many && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
                return RelationshipType.ManyToMany;

            throw new Exception(string.Format("The relationship type from [{0}] to [{1}] could not be determined.", associationEnd.Class.Name, associationEnd.OtherEnd().Class.Name));
        }

        public static string MultiplicityString(this IAssociationEnd associationEnd)
        {
            if (associationEnd.MaxMultiplicity == "*")
                return "*";
            if (associationEnd.MaxMultiplicity == associationEnd.MinMultiplicity)
                return associationEnd.MinMultiplicity;

            return associationEnd.MinMultiplicity + ".." + associationEnd.MaxMultiplicity;
        }

        public static string RelationshipString(this IAssociation association)
        {
            return string.Format("{0}->{1}", association.SourceEnd.MultiplicityString(), association.TargetEnd.MultiplicityString());
        }

        public static string IdentifierType(this IClass obj)
        {
            return obj.Name.ToPascalCase() + "Id";
        }

        public static string IdentifierName(this IAssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Name))
            {
                return associationEnd.Class.IdentifierType();
            }
            else
            {
                return associationEnd.Name.ToPascalCase() + "Id";
            }
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
