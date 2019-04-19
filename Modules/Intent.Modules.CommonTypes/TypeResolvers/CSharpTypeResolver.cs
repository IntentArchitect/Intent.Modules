using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.TypeResolvers;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public class CSharpTypeResolver : TypeResolverBase, ITypeResolver
    {

        protected override string ResolveType(ITypeReference typeInfo, string collectionType = null)
        {
            var result = typeInfo.Name;
            if (typeInfo.Stereotypes.Any(x => x.Name == "C#"))
            {
                string typeName = typeInfo.GetStereotypeProperty<string>("C#", "Type");
                string @namespace = typeInfo.GetStereotypeProperty<string>("C#", "Namespace");

                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;

                if (typeInfo.IsNullable && (typeInfo.Type == ReferenceType.Enum || (typeInfo.Type == ReferenceType.DataType && typeInfo.GetStereotypeProperty("C#", "IsPrimitive", false))))
                {
                    result += "?";
                }
            }
            else
            {
                if (typeInfo.IsNullable && typeInfo.Type == ReferenceType.Enum)
                {
                    result += "?";
                }
            }

            if (typeInfo.IsCollection)
            {
                result = $"{collectionType ?? nameof(IEnumerable)}<{result}>";
            }

            return result;
        }
    }
}
