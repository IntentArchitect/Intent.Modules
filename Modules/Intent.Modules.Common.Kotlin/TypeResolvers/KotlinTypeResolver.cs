using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Kotlin.TypeResolvers
{
    /// <summary>
    /// Kotlin specialization of <see cref="TypeResolverBase"/>.
    /// </summary>
    public class KotlinTypeResolver : TypeResolverBase, ITypeResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="KotlinTypeResolver"/>.
        /// </summary>
        public KotlinTypeResolver() : base(defaultContext: new KotlinTypeResolverContext())
        {
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new KotlinTypeResolverContext();
        }

        private class KotlinTypeResolverContext : TypeResolverContextBase<KotlinCollectionFormatter, KotlinResolvedTypeInfo>
        {
            private static readonly Dictionary<string, string> PrimitivesTypeMap = new()
            {
                ["byte"] = "Byte",
                ["short"] = "Short",
                ["int"] = "Int",
                ["long"] = "Long",
                ["float"] = "Float",
                ["double"] = "Double",
                ["bool"] = "Boolean",
                ["char"] = "Char"
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

            public KotlinTypeResolverContext()
                : base(KotlinCollectionFormatter.GetOrCreate("List<{0}>"), TypeResolution.DefaultNullableFormatter.Instance)
            {
            }

            protected override KotlinResolvedTypeInfo Get(IClassProvider classProvider)
            {
                return KotlinResolvedTypeInfo.Create(
                    name: classProvider.ClassName,
                    package: classProvider.Namespace,
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    template: classProvider,
                    genericTypeParameters: null);
            }

            protected override KotlinResolvedTypeInfo Get(
                IResolvedTypeInfo resolvedTypeInfo,
                IEnumerable<ITypeReference> genericTypeParameters,
                KotlinCollectionFormatter collectionFormatter)
            {
                return KotlinResolvedTypeInfo.Create(
                    resolvedTypeInfo: resolvedTypeInfo,
                    genericTypeParameters: genericTypeParameters
                        .Select(type => Get(type, collectionFormatter))
                        .ToArray());
            }

            protected override KotlinResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
            {
                var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                    ? KotlinCollectionFormatter.GetOrCreate(collectionFormat)
                    : null;

                return Get(typeInfo, collectionFormatter);
            }

            protected override KotlinResolvedTypeInfo ResolveType(
                ITypeReference typeReference,
                INullableFormatter nullableFormatter,
                KotlinCollectionFormatter collectionFormatter)
            {
                IReadOnlyList<KotlinResolvedTypeInfo> ResolveGenericTypeParameters(IEnumerable<ITypeReference> genericTypeParameters)
                {
                    return genericTypeParameters
                        .Select(type => Get(type, collectionFormatter))
                        .ToArray();
                }

                if (typeReference.Element == null)
                {
                    return KotlinResolvedTypeInfo.Create(
                        name: "void",
                        package: string.Empty,
                        isPrimitive: true,
                        isNullable: false,
                        isCollection: false,
                        typeReference: typeReference,
                        template: null);
                }

                if (typeReference.Element.HasStereotype("Kotlin"))
                {
                    var name = typeReference.Element.GetStereotypeProperty<string>("Kotlin", "Type");
                    var package = typeReference.Element.GetStereotypeProperty<string>("Kotlin", "Namespace");

                    var lastIndexOfPeriod = name.LastIndexOf('.');
                    if (lastIndexOfPeriod >= 0)
                    {
                        package = string.IsNullOrWhiteSpace(package)
                            ? name[..lastIndexOfPeriod]
                            : $"{package}.{name[..lastIndexOfPeriod]}";
                        name = name[(lastIndexOfPeriod + 1)..];
                    }

                    return KotlinResolvedTypeInfo.Create(
                        name: name,
                        package: package,
                        isPrimitive: typeReference.Element.GetStereotypeProperty("Kotlin", "Is Primitive", false),
                        isNullable: typeReference.IsNullable,
                        isCollection: typeReference.IsCollection,
                        typeReference: typeReference,
                        genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
                }

                if (PrimitivesTypeMap.TryGetValue(typeReference.Element.Name, out var primitiveTypeName))
                {
                    return KotlinResolvedTypeInfo.Create(
                        name: primitiveTypeName,
                        package: string.Empty,
                        isPrimitive: !typeReference.IsNullable,
                        isNullable: typeReference.IsNullable,
                        isCollection: false,
                        typeReference: typeReference);
                }

                if (ObjectsTypeMap.TryGetValue(typeReference.Element.Name, out var objectType))
                {
                    return KotlinResolvedTypeInfo.Create(
                        name: objectType.TypeName,
                        package: objectType.Package,
                        isPrimitive: false,
                        isNullable: typeReference.IsNullable,
                        isCollection: false,
                        typeReference: typeReference);
                }

                if (typeReference.Element.Name == "binary")
                {
                    return KotlinResolvedTypeInfo.Create(
                        name: "ByteArray",
                        package: string.Empty,
                        isPrimitive: false,
                        isNullable: typeReference.IsNullable,
                        isCollection: true,
                        typeReference: typeReference);
                }

                return KotlinResolvedTypeInfo.Create(
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
