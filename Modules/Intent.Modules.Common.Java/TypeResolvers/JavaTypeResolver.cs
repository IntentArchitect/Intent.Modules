using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    public class JavaTypeResolver : TypeResolverBase, ITypeResolver
    {

        public JavaTypeResolver() : base(defaultContext: new JavaTypeResolverContext(new JavaTypeResolverOptions()))
        {
        }

        protected override ITypeResolverContext CreateContext()
        {
            return new JavaTypeResolverContext(new JavaTypeResolverOptions());
        }
    }

    public class JavaTypeResolverOptions
    {
        public bool ReturnsPrimitives { get; set; } = true;
    }

    public class JavaTypeResolverContext : TypeResolverContextBase
    {

        public JavaTypeResolverContext(JavaTypeResolverOptions options) : base(new CollectionFormatter("{0}[]"))
        {
            Options = options;
        }

        public JavaTypeResolverOptions Options { get; set; }

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
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Boolean" : "boolean";
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
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Char" : "char";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "byte":
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Byte" : "byte";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "decimal":
                        result = "java.math.BigDecimal";
                        isPrimitive = false;
                        break;
                    case "double":
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Double" : "double";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "float":
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Float" : "float";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "short":
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Short" : "short";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "int":
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Integer" : "int";
                        isPrimitive = !typeInfo.IsNullable;
                        break;
                    case "long":
                        result = typeInfo.IsNullable || !Options.ReturnsPrimitives ? "Long" : "long";
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

            return new ResolvedTypeInfo(result, isPrimitive, null);
        }
    }
}
