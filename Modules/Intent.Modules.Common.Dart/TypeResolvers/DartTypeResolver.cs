using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Dart.TypeResolvers;

/// <summary>
/// Dart specialization of <see cref="TypeResolverBase"/>.
/// </summary>
public class DartTypeResolver : TypeResolverBase
{
    /// <summary>
    /// Creates a new instance of <see cref="DartTypeResolver"/>.
    /// </summary>
    public DartTypeResolver() : base(defaultContext: new DartTypeResolverContext())
    {
    }

    /// <inheritdoc />
    protected override ITypeResolverContext CreateContext()
    {
        return new DartTypeResolverContext();
    }

    private class DartTypeResolverContext : TypeResolverContextBase<DartCollectionFormatter, DartResolvedTypeInfo>
    {
        private static readonly Dictionary<string, string> TypeMap = new()
        {
            ["bool"] = "bool",
            ["byte"] = "int",
            ["char"] = "int",
            ["date"] = "DateTime",
            ["datetime"] = "DateTime",
            ["decimal"] = "double",
            ["double"] = "double",
            ["float"] = "double",
            ["guid"] = "String",
            ["int"] = "int",
            ["long"] = "int",
            ["object"] = "Object",
            ["short"] = "int",
            ["string"] = "String"
        };

        public DartTypeResolverContext()
            : base(DartCollectionFormatter.Create("List<{0}>"), DartNullableFormatter.Instance)
        {
        }

        protected override DartResolvedTypeInfo Get(IClassProvider classProvider)
        {
            return DartResolvedTypeInfo.Create(
                name: classProvider.ClassName,
                importSource: null,
                isPrimitive: false,
                isNullable: false,
                isCollection: false,
                typeReference: null,
                template: classProvider,
                genericTypeParameters: null);
        }

        protected override DartResolvedTypeInfo Get(
            IResolvedTypeInfo resolvedTypeInfo,
            IEnumerable<ITypeReference> genericTypeParameters,
            DartCollectionFormatter collectionFormatter)
        {
            return DartResolvedTypeInfo.Create(
                resolvedTypeInfo: resolvedTypeInfo,
                genericTypeParameters: genericTypeParameters
                    .Select(type => Get(type, collectionFormatter))
                    .ToArray());
        }

        protected override DartResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                ? DartCollectionFormatter.Create(collectionFormat)
                : null;

            return Get(typeInfo, collectionFormatter);
        }

        protected override DartResolvedTypeInfo ResolveType(
            ITypeReference typeReference,
            INullableFormatter nullableFormatter,
            DartCollectionFormatter collectionFormatter)
        {
            IReadOnlyList<DartResolvedTypeInfo> ResolveGenericTypeParameters(IEnumerable<ITypeReference> genericTypeParameters)
            {
                return genericTypeParameters
                    .Select(type => Get(type, collectionFormatter))
                    .ToArray();
            }

            if (typeReference.Element == null)
            {
                return DartResolvedTypeInfo.Create(
                    name: "void",
                    importSource: null,
                    isPrimitive: true,
                    isNullable: false,
                    isCollection: false,
                    typeReference: typeReference,
                    nullableFormatter: nullableFormatter,
                    template: null);
            }

            if (typeReference.Element.HasStereotype("Dart"))
            {
                var name = typeReference.Element.GetStereotypeProperty<string>("Dart", "Name");
                var importSource = typeReference.Element.GetStereotypeProperty<string>("Dart", "Import Source");

                return DartResolvedTypeInfo.Create(
                    name: !string.IsNullOrWhiteSpace(name) ? name : typeReference.Element.Name,
                    importSource: !string.IsNullOrWhiteSpace(importSource) ? importSource : null,
                    isPrimitive: typeReference.Element.GetStereotypeProperty("Dart", "Is Primitive", false),
                    isNullable: typeReference.IsNullable,
                    isCollection: typeReference.IsCollection,
                    typeReference: typeReference,
                    nullableFormatter: nullableFormatter,
                    genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
            }

            if (TypeMap.TryGetValue(typeReference.Element.Name, out var typeName))
            {
                return DartResolvedTypeInfo.Create(
                    name: typeName,
                    importSource: null,
                    isPrimitive: !typeReference.IsNullable,
                    isNullable: typeReference.IsNullable,
                    isCollection: false,
                    typeReference: typeReference,
                    nullableFormatter: nullableFormatter);
            }

            if (typeReference.Element.Name == "binary")
            {
                DartResolvedTypeInfo.CreateForCollection(
                    forResolvedType: DartResolvedTypeInfo.Create(
                        name: "int",
                        importSource: null,
                        isPrimitive: true,
                        isNullable: false,
                        isCollection: false,
                        typeReference: typeReference,
                        nullableFormatter: nullableFormatter),
                    isNullable: typeReference.IsNullable,
                    nullableFormatter: nullableFormatter);
            }

            return DartResolvedTypeInfo.Create(
                name: typeReference.Element.Name,
                importSource: null,
                isPrimitive: false,
                isNullable: typeReference.IsNullable,
                isCollection: false,
                typeReference: typeReference,
                nullableFormatter: nullableFormatter,
                genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
        }
    }
}