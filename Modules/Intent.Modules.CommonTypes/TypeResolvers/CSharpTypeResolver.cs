using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public class CSharpTypeResolver : TypeResolverBase, ITypeResolver
    {

        protected override string ResolveType(ITypeReference typeInfo)
        {
            var result = typeInfo.Name;
            if (typeInfo.Stereotypes.Any(x => x.Name == "C#"))
            {
                string typeName = typeInfo.Stereotypes.GetPropertyValue<string>("C#", "Type");
                string @namespace = typeInfo.Stereotypes.GetPropertyValue<string>("C#", "Namespace");

                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;

                if (typeInfo.IsNullable && (typeInfo.Type == ReferenceType.Enum || (typeInfo.Type == ReferenceType.DataType && typeInfo.Stereotypes.GetPropertyValue("C#", "IsPrimitive", false))))
                {
                    result += "?";
                }
            }

            return result;
        }
    }
}
