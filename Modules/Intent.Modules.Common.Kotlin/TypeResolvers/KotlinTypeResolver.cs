using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Kotlin.TypeResolvers
{
    public class KotlinTypeResolver : TypeResolverBase, ITypeResolver
    {

        public KotlinTypeResolver() : base(defaultContext: new KotlinTypeResolverContext(new KotlinTypeResolverOptions()))
        {
        }

        protected override ITypeResolverContext CreateContext()
        {
            return new KotlinTypeResolverContext(new KotlinTypeResolverOptions());
        }
    }

    public class KotlinTypeResolverOptions
    {
        public bool ReturnsPrimitives { get; set; } = true;
    }

    public class KotlinTypeResolverContext : TypeResolverContextBase
    {

        public KotlinTypeResolverContext(KotlinTypeResolverOptions options) : base(new CollectionFormatter("List<{0}>"))
        {
            Options = options;
        }

        public KotlinTypeResolverOptions Options { get; set; }

        protected override string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes)
        {
            return $"{type.Name}<{string.Join(", ", genericTypes.Select(x => x.Name))}>";
        }

        protected override ResolvedTypeInfo ResolveType(ITypeReference typeInfo)
        {
            if (typeInfo.Element == null)
            {
                return new ResolvedTypeInfo("void", false, null);
            }
            var result = typeInfo.Element.Name;
            var isPrimitive = false;
            if (typeInfo.Element.HasStereotype("Kotlin"))
            {
                string typeName = typeInfo.Element.GetStereotypeProperty<string>("Kotlin", "Type");
                string @namespace = typeInfo.Element.GetStereotypeProperty<string>("Kotlin", "Namespace");
                isPrimitive = typeInfo.Element.GetStereotypeProperty<bool>("Kotlin", "Is Primitive", false);
                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;
            }
            else
            {
                isPrimitive = true;
                switch (typeInfo.Element.Name)
                {
                    case "bool":
                        result = "Boolean";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "date":
                        result = $"java.time.LocalDate";
                        isPrimitive = false;
                        break;
                    case "datetime":
                        result = $"java.time.LocalDateTime";
                        isPrimitive = false;
                        break;
                    case "char":
                        result = "Char";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "byte":
                        result = "Byte";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "decimal":
                        result = $"java.math.BigDecimal{(typeInfo.IsNullable ? " ? " : "")}";
                        isPrimitive = false;
                        break;
                    case "double":
                        result = $"Double";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "float":
                        result = $"Float";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "short":
                        result = $"Short";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "int":
                        result = $"Integer";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "long":
                        result = $"Long";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "datetimeoffset":
                        result = $"java.time.OffsetDateTime";
                        isPrimitive = false;
                        break;
                    case "binary":
                        result = "byte[]";
                        isPrimitive = false;
                        break;
                    case "object":
                        result = $"Object";
                        isPrimitive = false;
                        break;
                    case "guid":
                        result = $"java.util.UUID";
                        isPrimitive = false;
                        break;
                    case "string":
                        result = $"String";
                        isPrimitive = false;
                        break;
                }

                result = !string.IsNullOrWhiteSpace(result)
                    ? result
                    : typeInfo.Element.Name;
            }

            if (typeInfo.IsNullable)
            {
                result += "?";
            }

            return new ResolvedTypeInfo(result, isPrimitive, null);
        }
    }
}
