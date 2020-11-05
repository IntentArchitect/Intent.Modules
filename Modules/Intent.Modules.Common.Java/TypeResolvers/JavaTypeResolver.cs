using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    public class JavaTypeResolver : TypeResolverBase, ITypeResolver
    {
        public override string DefaultCollectionFormat { get; set; } = "{0}[]";

        protected override IResolvedTypeInfo ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            if (typeInfo.Element == null)
            {
                return new ResolvedTypeInfo("void", false, null);
            }
            var result = typeInfo.Element.Name;
            var isPrimitive = false;
            if (typeInfo.Element.HasStereotype("Java"))
            {
                string typeName = typeInfo.Element.GetStereotypeProperty<string>("Java", "Type");
                string @namespace = typeInfo.Element.GetStereotypeProperty<string>("Java", "Namespace");
                isPrimitive = typeInfo.Element.GetStereotypeProperty<bool>("Java", "Is Primitive", false);
                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;
            }
            else
            {
                isPrimitive = true;
                switch (typeInfo.Element.Name)
                {
                    case "bool":
                        result = typeInfo.IsNullable ? "Boolean" : "boolean";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "date":
                        result = "java.time.LocalDate";
                        isPrimitive = false;
                        break;
                    case "datetime":
                        result = "java.time.LocalDateTime";
                        isPrimitive = false;
                        break;
                    case "char":
                        result = typeInfo.IsNullable ? "Char" : "char";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "byte":
                        result = typeInfo.IsNullable ? "Byte" : "byte";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "decimal":
                        result = "java.math.BigDecimal";
                        isPrimitive = false;
                        break;
                    case "double":
                        result = typeInfo.IsNullable ? "Double" : "double";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "float":
                        result = typeInfo.IsNullable ? "Float" : "float";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "short":
                        result = typeInfo.IsNullable ? "Short" : "short";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "int":
                        result = typeInfo.IsNullable ? "Integer" : "int";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "long":
                        result = typeInfo.IsNullable ? "Long" : "long";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "datetimeoffset":
                        result = "java.time.OffsetDateTime";
                        isPrimitive = false;
                        break;
                    case "binary":
                        result = "byte[]";
                        isPrimitive = false;
                        break;
                    case "object":
                        result = "Object";
                        isPrimitive = false;
                        break;
                    case "guid":
                        result = "java.util.UUID";
                        isPrimitive = false;
                        break;
                    case "string":
                        result = "String";
                        isPrimitive = false;
                        break;
                }

                result = !string.IsNullOrWhiteSpace(result)
                    ? result
                    : typeInfo.Element.Name;
            }

            if (typeInfo.IsCollection)
            {
                isPrimitive = false;
                result = string.Format(collectionFormat ?? DefaultCollectionFormat, result);
            }

            return new ResolvedTypeInfo(result, isPrimitive, null);
        }
    }
}
