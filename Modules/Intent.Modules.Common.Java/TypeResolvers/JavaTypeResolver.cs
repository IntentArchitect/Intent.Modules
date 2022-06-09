using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
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
        public JavaTypeResolver() : base(defaultContext: new JavaTypeResolverContext())
        {
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new JavaTypeResolverContext();
        }

        private class JavaTypeResolverContext : TypeResolverContextBase
        {
            private static readonly Dictionary<string, string> PrimitivesTypeMap = new()
            {
                ["byte"] = "byte",
                ["short"] = "short",
                ["int"] = "int",
                ["long"] = "long",
                ["float"] = "float",
                ["double"] = "double",
                ["bool"] = "boolean",
                ["char"] = "char"
            };

            private static readonly Dictionary<string, (string Package, string TypeName)> ObjectsTypeMap = new()
            {
                ["string"] = (string.Empty, "String"),
                ["object"] = (string.Empty, "Object"),
                ["datetime"] = ("java.time", "LocalDateTime"),
                ["date"] = ("java.time", "LocalDate"),
                ["decimal"] = ("java.math", "BigDecimal"),
                ["datetimeoffset"] = ("java.time", "OffsetDateTime"),
                ["guid"] = ("java.util", "UUID")
            };

            public JavaTypeResolverContext()
                : base(JavaCollectionFormatter.GetOrCreate("{0}[]"), TypeResolution.DefaultNullableFormatter.Instance)
            {
            }

            public override IResolvedTypeInfo Get(IClassProvider classProvider)
            {
                return JavaResolvedTypeInfo.Create(
                    name: classProvider.ClassName,
                    package: classProvider.Namespace,
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    template: classProvider,
                    genericTypeParameters: null);
            }

            protected override IResolvedTypeInfo Get(IResolvedTypeInfo resolvedTypeInfo)
            {
                return JavaResolvedTypeInfo.Create(resolvedTypeInfo);
            }

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

            private static JavaResolvedTypeInfo ResolveTypeInternal(ITypeReference typeReference)
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

                if (PrimitivesTypeMap.TryGetValue(typeReference.Element.Name, out var primitiveTypeName))
                {
                    return JavaResolvedTypeInfo.Create(
                        name: primitiveTypeName,
                        package: string.Empty,
                        isPrimitive: true,
                        isNullable: typeReference.IsNullable,
                        isCollection: false,
                        typeReference: typeReference);
                }

                if (ObjectsTypeMap.TryGetValue(typeReference.Element.Name, out var objectType))
                {
                    return JavaResolvedTypeInfo.Create(
                        name: objectType.TypeName,
                        package: objectType.Package,
                        isPrimitive: false,
                        isNullable: typeReference.IsNullable,
                        isCollection: false,
                        typeReference: typeReference);
                }

                if (typeReference.Element.Name == "binary")
                {
                    return JavaResolvedTypeInfo.CreateForArray(
                        forResolvedType: JavaResolvedTypeInfo.Create(
                            name: "byte",
                            package: string.Empty,
                            isPrimitive: true,
                            isNullable: false,
                            isCollection: false,
                            typeReference: typeReference),
                        isNullable: typeReference.IsNullable,
                        arrayDimensionCount: 1);
                }

                return JavaResolvedTypeInfo.Create(
                    name: typeReference.Element.Name,
                    package: string.Empty,
                    isPrimitive: false,
                    isNullable: typeReference.IsNullable,
                    isCollection: false,
                    typeReference: typeReference,
                    genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
            }
        }
    }
}
