using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common
{
    public static class TypeHelper
    {

        public static string GetDescription(this Type type)
        {
            var attribures = type.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attribures.Any())
            {
                return ((DescriptionAttribute)attribures[0]).Description;
            }
            return type.FullName;
        }

    }

    public static class MetadataExtensions
    {
        public static IList<IElement> GetParentPath(this IElement model)
        {
            List<IElement> result = new List<IElement>();

            var current = model.ParentElement;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.ParentElement;
            }
            return result;
        }

        public static IList<IElement> GetParentPath(this IStereotypeDefinition model)
        {
            List<IElement> result = new List<IElement>();

            var current = model.ParentElement;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.ParentElement;
            }
            return result;
        }
    }
}
