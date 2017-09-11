using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.MetaModel.Domain;
using ITypeReference = Intent.MetaModel.Domain.ITypeReference;

namespace Intent.Packages.Entities.Templates
{
    public static class DomainEntityModelExtensions
    {
        public static T GetProperty<T>(this IEnumerable<IStereotype> stereotypes, string stereoTypeName, string tagName, T defaultValue = default(T))
        {
            foreach (var s in stereotypes)
            {
                if (s.Name == stereoTypeName)
                {
                    foreach (var tag in s.Properties)
                    {
                        if (tag.Key == tagName)
                        {
                            if (!string.IsNullOrWhiteSpace(tag.Value))
                            {
                                return (T)Convert.ChangeType(tag.Value, typeof(T));
                            }
                        }
                    }
                }
            }
            return defaultValue;
        }

        public static string TypeName(this IAttribute attribute)
        {
            return attribute.Type.ConvertType();
        }

        public static string Type(this IAttribute attribute, IEnumerable<IAttibuteTypeConverter> decorators)
        {
            return decorators.Select(x => x.ConvertAttributeType(attribute)).FirstOrDefault(x => x != null) ?? attribute.Type.ConvertType();
        }

        public static string Name(this IAssociationEnd associationEnd)
        {
            return associationEnd.Name ?? associationEnd.Class.Name;
        }

        public static string Type(this IAssociationEnd associationEnd)
        {
            return associationEnd.Type("", false);
        }

        public static string Type(this IAssociationEnd associationEnd, string suffix)
        {
            return associationEnd.Type("", suffix, false);
        }

        public static string Type(this IAssociationEnd associationEnd, string prefix, string suffix)
        {
            return associationEnd.Type(prefix, suffix, false);
        }


        public static string Type(this IAssociationEnd associationEnd, string suffix, bool readOnly)
        {
            return associationEnd.Type("", suffix, readOnly);
        }

        public static string Type(this IAssociationEnd associationEnd, string prefix, string suffix, bool readOnly)
        {
            if (associationEnd.Multiplicity == Multiplicity.Many)
            {
                if (readOnly)
                {
                    return "IEnumerable<" + prefix + associationEnd.Class.Name + suffix + ">";
                }
                else
                {
                    return "ICollection<" + prefix + associationEnd.Class.Name + suffix + ">";
                }
            }
            return prefix + associationEnd.Class.Name + suffix;
        }

        public static string ConvertType(this ITypeReference type)
        {
            var nullableString = (type.IsNullable) ? "?" : "";
            switch (type.Name)
            {
                case "string":
                    return "string";
                case "date":
                    return "System.DateTime" + nullableString;
                case "datetime":
                    return "System.DateTime" + nullableString;
                case "guid":
                    return "System.Guid" + nullableString;
                case "int":
                    return "int" + nullableString;
                case "decimal":
                    return "decimal" + nullableString;
                case "bool":
                case "boolean":
                    return "bool" + nullableString;
                case "binary":
                    return "byte[]";
                default:
                    if (type.Type == DomainType.Enum)
                    {
                        return type.Name + nullableString;
                    }
                    return type.Name;
            }
        }
    }

    public interface IAttibuteTypeConverter
    {
        string ConvertAttributeType(IAttribute attribute);
    }
}