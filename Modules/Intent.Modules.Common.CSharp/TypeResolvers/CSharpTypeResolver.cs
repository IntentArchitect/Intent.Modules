using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    public class CSharpTypeResolver : TypeResolverBase, ITypeResolver
    {
        private readonly ICollectionFormatter _defaultCollectionFormatter;
        private readonly INullableFormatter _defaultNullableFormatter;

        public CSharpTypeResolver(ICollectionFormatter defaultCollectionFormatter, INullableFormatter defaultNullableFormatter) : base(new CSharpTypeResolverContext(defaultCollectionFormatter, defaultNullableFormatter))
        {
            _defaultCollectionFormatter = defaultCollectionFormatter;
            _defaultNullableFormatter = defaultNullableFormatter;
        }

        protected override ITypeResolverContext CreateContext()
        {
            return new CSharpTypeResolverContext(_defaultCollectionFormatter, _defaultNullableFormatter);
        }
    }


    public class CSharpTypeResolverContext : TypeResolverContextBase
    {
        public CSharpTypeResolverContext(ICollectionFormatter collectionFormatter, INullableFormatter nullableFormatter) : base(collectionFormatter, nullableFormatter)
        {
        }

        protected override string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes)
        {
            return $"{type.Name}<{string.Join(", ", genericTypes.Select(x => x.Name))}>";
        }

        protected override ResolvedTypeInfo ResolveType(ITypeReference typeInfo)
        {
            if (typeInfo.Element == null)
            {
                return new ResolvedTypeInfo("void", true, typeInfo.IsNullable, typeInfo.IsCollection, typeInfo, null);
            }

            var result = typeInfo.Element.Name;
            var isPrimitive = true;
            var isCollection = typeInfo.IsCollection;
            if (typeInfo.Element.Stereotypes.Any(x => x.Name == "C#"))
            {
                string typeName = typeInfo.Element.GetStereotypeProperty<string>("C#", "Type", typeInfo.Element.Name);
                string @namespace = typeInfo.Element.GetStereotypeProperty<string>("C#", "Namespace");
                isPrimitive = typeInfo.Element.GetStereotypeProperty("C#", "Is Primitive", true);
                isCollection = typeInfo.Element.GetStereotypeProperty("C#", "Is Collection", typeInfo.IsCollection);
                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;

                // Moved to NullFormatter
                //if (typeInfo.IsNullable && isPrimitive)
                //{
                //    result += "?";
                //}
            }
            else
            {
                switch (typeInfo.Element.Name)
                {
                    case "bool":
                        result = "bool";
                        break;
                    case "date":
                    case "datetime":
                        result = "System.DateTime";
                        break;
                    case "char":
                        result = "char";
                        break;
                    case "byte":
                        result = "byte";
                        break;
                    case "decimal":
                        result = "decimal";
                        break;
                    case "double":
                        result = "double";
                        break;
                    case "float":
                        result = "float";
                        break;
                    case "short":
                        result = "short";
                        break;
                    case "int":
                        result = "int";
                        break;
                    case "long":
                        result = "long";
                        break;
                    case "datetimeoffset":
                        result = "System.DateTimeOffset";
                        break;
                    case "binary":
                        result = "byte[]";
                        isPrimitive = false;
                        break;
                    case "object":
                        result = "object";
                        break;
                    case "guid":
                        result = "System.Guid";
                        break;
                    case "string":
                        result = "string";
                        isPrimitive = false;
                        break;
                    default:
                        isPrimitive = false;
                        break;
                }

                // Moved to the NullFormatter:
                //if (typeInfo.IsNullable && (isPrimitive || _project.IsNullableAwareContext() || typeInfo.Element.SpecializationType.Equals("Enum", StringComparison.InvariantCultureIgnoreCase)))
                //{
                //    result += "?";
                //}
            }

            return new ResolvedTypeInfo(result, isPrimitive, typeInfo.IsNullable, isCollection, typeInfo, null);
        }
    }
}
