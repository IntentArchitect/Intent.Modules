using System;
using System.Collections;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
{
    public class CSharpTypeResolver : TypeResolverBase, ITypeResolver
    {

        protected override string ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            var result = ((ITypeReference)typeInfo).Name;
            if (typeInfo.Stereotypes.Any(x => x.Name == "C#"))
            {
                string typeName = typeInfo.GetStereotypeProperty<string>("C#", "Type", typeInfo.Name);
                string @namespace = typeInfo.GetStereotypeProperty<string>("C#", "Namespace");

                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;

                if (typeInfo.IsNullable && (typeInfo.SpecializationType.Equals("Enum",StringComparison.InvariantCultureIgnoreCase) || (typeInfo.GetStereotypeProperty("C#", "IsPrimitive", false))))
                {
                    result += "?";
                }
            }
            else
            {
                if (typeInfo.SpecializationType.Equals("Enum", StringComparison.InvariantCultureIgnoreCase))
                {
                    result += "?";
                }
            }

            if (typeInfo.IsCollection)
            {
                result = string.Format(collectionFormat ?? "IEnumerable<{0}>", result);
            }

            return result;
        }
    }
}
