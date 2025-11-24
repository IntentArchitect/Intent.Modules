using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.TypeResolvers
{
    /// <summary>
    /// TypeScript specialization of <see cref="TypeResolverBase"/>.
    /// </summary>
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeScriptTypeResolver"/>.
        /// </summary>
        public TypeScriptTypeResolver() : base(new TypeScriptTypeResolverContext(TypescriptCollectionFormatter.Create("{0}[]"), TypeResolution.DefaultNullableFormatter.Instance))
        {
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new TypeScriptTypeResolverContext(TypescriptCollectionFormatter.Create("{0}[]"), TypeResolution.DefaultNullableFormatter.Instance);
        }

        private class TypeScriptTypeResolverContext : TypeResolverContextBase<TypescriptCollectionFormatter, TypescriptResolvedTypeInfo>
        {
            public TypeScriptTypeResolverContext(
                TypescriptCollectionFormatter defaultCollectionFormatter,
                INullableFormatter defaultNullableFormatter)
                : base(
                    defaultCollectionFormatter,
                    defaultNullableFormatter)
            {
            }

            protected override TypescriptResolvedTypeInfo Get(IClassProvider classProvider)
            {
                return TypescriptResolvedTypeInfo.Create(
                    name: classProvider.ClassName,
                    @namespace: classProvider.Namespace,
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    nullableFormatter: null,
                    template: classProvider,
                    genericTypeParameters: null);
            }

            protected override TypescriptResolvedTypeInfo Get(
                IResolvedTypeInfo resolvedTypeInfo,
                IEnumerable<ITypeReference> genericTypeParameters,
                TypescriptCollectionFormatter collectionFormatter)
            {
                return TypescriptResolvedTypeInfo.Create(
                    resolvedTypeInfo: resolvedTypeInfo,
                    genericTypeParameters: genericTypeParameters
                        .Select(type => Get(type, collectionFormatter))
                        .ToArray());
            }

            protected override TypescriptResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
            {
                var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                    ? TypescriptCollectionFormatter.Create(collectionFormat)
                    : null;

                return Get(typeInfo, collectionFormatter);
            }

            protected override TypescriptResolvedTypeInfo ResolveType(
                ITypeReference typeReference,
                INullableFormatter nullableFormatter,
                TypescriptCollectionFormatter collectionFormatter)
            {
                if (typeReference.Element == null)
                {
                    return TypescriptResolvedTypeInfo.Create(
                        name: "void",
                        @namespace: string.Empty,
                        isPrimitive: true,
                        isNullable: false,
                        isCollection: false,
                        typeReference: typeReference,
                        nullableFormatter: nullableFormatter,
                        template: null);
                }
                string name = null;
                bool isPrimitive = false;

                if (typeReference.Element.HasStereotype("TypeScript"))
                {
                    name = typeReference.Element.GetStereotypeProperty<string>("TypeScript", "Type");
                }
                else
                {
                    isPrimitive = true;
                    switch (typeReference.Element.Name)
                    {
                        case "bool":
                            name = "boolean";
                            break;
                        case "date":
                        case "datetime":
                            name = "Date";
                            break;
                        case "byte":
                        case "decimal":
                        case "double":
                        case "float":
                        case "short":
                        case "int":
                        case "long":
                            name = "number";
                            break;
                        case "datetimeoffset":
                        case "binary":
                        case "object":
                            name = "any";
                            break;
                        case "char":
                        case "guid":
                        case "string":
                            name = "string";
                            break;
                    }

                    name = !string.IsNullOrWhiteSpace(name)
                        ? name
                        : typeReference.Element.Name;
                }

                return TypescriptResolvedTypeInfo.Create(
                    name: name,
                    @namespace: string.Empty,    
                    isPrimitive: isPrimitive,
                    typeReference: typeReference,
                    template: null,
                    nullableFormatter: nullableFormatter,
                    genericTypeParameters: (typeReference.GenericTypeParameters ?? Enumerable.Empty<ITypeReference>())
                        .Select(Get)
                        .ToArray()
                    );
            }
        }
    }
}
