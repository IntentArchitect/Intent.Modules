using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.EntityFramework.Templates
{
    public static class Extensions
    {
        public static string Name(this AssociationEndModel associationEnd)
        {
            //if (string.IsNullOrEmpty(associationEnd.Name))
            //{
            //    var className = associationEnd.Class.Name;
            //    if (associationEnd.MaxMultiplicity == "*" || int.Parse(associationEnd.MaxMultiplicity) > 1)
            //    {
            //        return className.EndsWith("y") ? className.Substring(0, className.Length - 1) + "ies" : string.Format("{0}s", className);
            //    }
            //    return associationEnd.Class.Name;
            //}

            return associationEnd.Name;
        }

        public static RelationshipType Relationship(this AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsCollection && !associationEnd.OtherEnd().IsCollection)
                return RelationshipType.OneToOne;
            if (!associationEnd.IsCollection && associationEnd.OtherEnd().IsCollection)
                return RelationshipType.OneToMany;
            if (associationEnd.IsCollection && !associationEnd.OtherEnd().IsCollection)
                return RelationshipType.ManyToOne;
            if (associationEnd.IsCollection && associationEnd.OtherEnd().IsCollection)
                return RelationshipType.ManyToMany;

            throw new Exception($"The relationship type from [{associationEnd.Element.Name}] to [{associationEnd.OtherEnd().Element.Name}] could not be determined.");
        }

        public static string MultiplicityString(this AssociationEndModel associationEnd)
        {
            if (associationEnd.IsCollection)
            {
                return associationEnd.IsNullable ? "0..*" : "1..*";
            }
            else
            {
                return associationEnd.IsNullable ? "0..1" : "1";
            }
        }

        public static string RelationshipString(this AssociationModel association)
        {
            return string.Format("{0}->{1}", association.SourceEnd.MultiplicityString(), association.TargetEnd.MultiplicityString());
        }

        public static string IdentifierType(this ClassModel obj)
        {
            return obj.Name.ToPascalCase() + "Id";
        }

        public static string IdentifierName(this AssociationEndModel associationEnd)
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
