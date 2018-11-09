using Intent.MetaModel.Common;
using Intent.MetaModel.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Templates
{
    public static class DomainEntityModelExtensions
    {
        //public static T GetProperty<T>(this IEnumerable<IStereotype> stereotypes, string stereoTypeName, string tagName, T defaultValue = default(T))
        //{
        //    foreach (var s in stereotypes)
        //    {
        //        if (s.Name == stereoTypeName)
        //        {
        //            foreach (var tag in s.Properties)
        //            {
        //                if (tag.Key == tagName)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(tag.Value))
        //                    {
        //                        return (T)Convert.ChangeType(tag.Value, typeof(T));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return defaultValue;
        //}

        //public static string TypeName(this IAttribute attribute)
        //{
        //    return attribute.Type.ConvertType();
        //}

        public static string ConvertType<T>(this IntentRoslynProjectItemTemplateBase<T> template, ITypeReference type, string context = null, string collectionType = nameof(IEnumerable))
        {
            var returnType = template.NormalizeNamespace(template.Types.Get(type, context));
            if (type.IsCollection && type.Type != ReferenceType.ClassType) // GCB - ClassType automatically adds the collectiont type. This disparity is a smell..
            {
                returnType = $"{collectionType}<{returnType}>";
            }
            return returnType;
        }

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

        //public static string Type(this IAssociationEnd associationEnd)
        //{
        //    return associationEnd.Type("", false);
        //}

        //public static string Type(this IAssociationEnd associationEnd, string suffix)
        //{
        //    return associationEnd.Type("", suffix, false);
        //}

        //public static string Type(this IAssociationEnd associationEnd, string prefix, string suffix)
        //{
        //    return associationEnd.Type(prefix, suffix, false);
        //}


        //public static string Type(this IAssociationEnd associationEnd, string suffix, bool readOnly)
        //{
        //    return associationEnd.Type("", suffix, readOnly);
        //}

        //public static string Type(this IAssociationEnd associationEnd, string prefix, string suffix, bool readOnly)
        //{
        //    if (associationEnd.Multiplicity == Multiplicity.Many)
        //    {
        //        if (readOnly)
        //        {
        //            return "IEnumerable<" + prefix + associationEnd.Class.Name + suffix + ">";
        //        }
        //        else
        //        {
        //            return "ICollection<" + prefix + associationEnd.Class.Name + suffix + ">";
        //        }
        //    }
        //    return prefix + associationEnd.Class.Name + suffix;
        //}
    }

    public interface IAttibuteTypeConverter
    {
        string ConvertAttributeType(IAttribute attribute);
    }
}