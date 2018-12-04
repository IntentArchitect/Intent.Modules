using System;
using System.ComponentModel;
using System.Linq;

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
}
