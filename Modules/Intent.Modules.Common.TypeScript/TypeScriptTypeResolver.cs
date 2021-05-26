using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.TypeScript
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {

        public TypeScriptTypeResolver() : base(new TypeScriptTypeResolverContext(new CollectionFormatter("{0}[]")))
        {
        }

        protected override ITypeResolverContext CreateContext()
        {
            return new TypeScriptTypeResolverContext(new CollectionFormatter("{0}[]"));
        }
    }
    public class TypeScriptTypeResolverContext : TypeResolverContextBase
    {
        protected override string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes)
        {
            return $"{type.Name}<{string.Join(", ", genericTypes.Select(x => x.Name))}>";
        }

        protected override ResolvedTypeInfo ResolveType(ITypeReference typeInfo)
        {
            string result = null;
            bool isPrimitive = false;
            if (typeInfo.Element.HasStereotype("TypeScript"))
            {
                result = typeInfo.Element.GetStereotypeProperty<string>("TypeScript", "Type");
            }
            else
            {
                isPrimitive = true;
                switch (typeInfo.Element.Name)
                {
                    case "bool":
                        result = "boolean";
                        break;
                    case "date":
                    case "datetime":
                        result = "Date";
                        break;
                    case "char":
                    case "byte":
                    case "decimal":
                    case "double":
                    case "float":
                    case "short":
                    case "int":
                    case "long":
                        result = "number";
                        break;
                    case "datetimeoffset":
                    case "binary":
                    case "object":
                        result = "any";
                        break;
                    case "guid":
                    case "string":
                        result = "string";
                        break;
                }

                result = !string.IsNullOrWhiteSpace(result)
                    ? result
                    : typeInfo.Element.Name;
            }

            return new ResolvedTypeInfo(result, isPrimitive, null);
        }

        public TypeScriptTypeResolverContext(ICollectionFormatter defaultCollectionFormatter) : base(defaultCollectionFormatter)
        {
        }
    }
}
