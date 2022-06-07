using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    /// <summary>
    /// Java specialization of <see cref="TypeResolverBase"/>.
    /// </summary>
    public class JavaTypeResolver : TypeResolverBase, ITypeResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="JavaTypeResolver"/>.
        /// </summary>
        public JavaTypeResolver() : base(defaultContext: new JavaTypeResolverContext(new JavaTypeResolverOptions()))
        {
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new JavaTypeResolverContext(new JavaTypeResolverOptions());
        }

        private class JavaTypeResolverContext : TypeResolverContextBase
        {

            public JavaTypeResolverContext(JavaTypeResolverOptions options) : base(new CollectionFormatter("{0}[]"), new DefaultNullableFormatter())
            {
                Options = options;
            }

            public JavaTypeResolverOptions Options { get; }

            public override IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
            {
                var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                    ? JavaCollectionFormatter.GetOrCreate(collectionFormat)
                    : null;

                return Get(typeInfo, collectionFormatter);
            }

            protected override IResolvedTypeInfo ResolveType(
                ITypeReference typeReference,
                INullableFormatter nullableFormatter)
            {
                return ResolveTypeInternal(typeReference);
            }

            private JavaResolvedTypeInfo ResolveTypeInternal(ITypeReference typeReference)
            {
                IReadOnlyList<JavaResolvedTypeInfo> ResolveGenericTypeParameters(IEnumerable<ITypeReference> genericTypeParameters)
                {
                    return genericTypeParameters.Select(ResolveTypeInternal).ToArray();
                }

                if (typeReference.Element == null)
                {
                    return JavaResolvedTypeInfo.Create(
                        name: "void",
                        package: string.Empty,
                        isPrimitive: true,
                        isNullable: false,
                        isCollection: false,
                        typeReference: typeReference,
                        template: null);
                }

                if (typeReference.Element.HasStereotype("Java"))
                {
                    var name = typeReference.Element.GetStereotypeProperty("Java", "Type", typeReference.Element.Name);
                    var package = typeReference.Element.GetStereotypeProperty("Java", "Package", string.Empty);

                    var lastIndexOfPeriod = name.LastIndexOf('.');
                    if (lastIndexOfPeriod >= 0)
                    {
                        package = string.IsNullOrWhiteSpace(package)
                            ? name[..lastIndexOfPeriod]
                            : $"{package}.{name[..lastIndexOfPeriod]}";
                        name = name[(lastIndexOfPeriod + 1)..];
                    }

                    return JavaResolvedTypeInfo.Create(
                        name: name,
                        package: package,
                        isPrimitive: typeReference.Element.GetStereotypeProperty("Java", "Is Primitive", false),
                        isNullable: typeReference.IsNullable,
                        isCollection: typeReference.IsCollection,
                        typeReference: typeReference,
                        genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
                }

                var result = typeReference.Element.Name;
                var isPrimitive = true;
                switch (typeReference.Element.Name)
                {
                    case "bool":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Boolean" : "boolean";
                        isPrimitive = !typeReference.IsNullable;
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
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Char" : "char";
                        isPrimitive = !typeReference.IsNullable;
                        break;
                    case "byte":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Byte" : "byte";
                        isPrimitive = !typeReference.IsNullable;
                        break;
                    case "decimal":
                        result = "java.math.BigDecimal";
                        isPrimitive = false;
                        break;
                    case "double":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Double" : "double";
                        isPrimitive = !typeReference.IsNullable;
                        break;
                    case "float":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Float" : "float";
                        isPrimitive = !typeReference.IsNullable;
                        break;
                    case "short":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Short" : "short";
                        isPrimitive = !typeReference.IsNullable;
                        break;
                    case "int":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Integer" : "int";
                        isPrimitive = !typeReference.IsNullable;
                        break;
                    case "long":
                        result = typeReference.IsNullable || !Options.ReturnsPrimitives ? "Long" : "long";
                        isPrimitive = !typeReference.IsNullable;
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
                    : typeReference.Element.Name;

                return new JavaResolvedTypeInfo(result, isPrimitive, typeReference, null);
            }
        }
    }

    public class JavaTypeResolverOptions
    {
        public bool ReturnsPrimitives { get; set; } = false;
    }
}
