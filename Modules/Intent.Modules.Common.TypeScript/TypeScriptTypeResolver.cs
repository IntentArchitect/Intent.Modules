using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.TypeScript
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        public override string DefaultCollectionFormat { get; set; } = "{0}[]";

        protected override IResolvedTypeInfo ResolveType(ITypeReference typeInfo, string collectionFormat = null)
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

            if (typeInfo.IsCollection)
            {
                result = string.Format(collectionFormat ?? DefaultCollectionFormat, result);
                isPrimitive = false;
            }

            return new ResolvedTypeInfo(result, isPrimitive, null);
        }

    }
}
