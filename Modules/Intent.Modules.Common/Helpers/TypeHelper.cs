using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory
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
