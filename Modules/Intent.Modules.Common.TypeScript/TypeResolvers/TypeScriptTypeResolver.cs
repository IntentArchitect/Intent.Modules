using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

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
        public TypeScriptTypeResolver() : base(new TypeScriptTypeResolverContext(CollectionFormatter.Create("{0}[]"), TypeResolution.DefaultNullableFormatter.Instance))
        {
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new TypeScriptTypeResolverContext(CollectionFormatter.Create("{0}[]"), TypeResolution.DefaultNullableFormatter.Instance);
        }

        private class TypeScriptTypeResolverContext : TypeResolverContextBase
        {
            public TypeScriptTypeResolverContext(
                ICollectionFormatter defaultCollectionFormatter,
                INullableFormatter defaultNullableFormatter)
                : base(
                    defaultCollectionFormatter,
                    defaultNullableFormatter)
            {
            }

            protected override IResolvedTypeInfo ResolveType(ITypeReference typeReference, INullableFormatter nullableFormatter)
            {
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
                        case "char":
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
                        case "guid":
                        case "string":
                            name = "string";
                            break;
                    }

                    name = !string.IsNullOrWhiteSpace(name)
                        ? name
                        : typeReference.Element.Name;
                }

                return ResolvedTypeInfo.Create(
                    name: name,
                    isPrimitive: isPrimitive,
                    typeReference: typeReference,
                    template: null,
                    nullableFormatter: nullableFormatter,
                    collectionFormatter: null,
                    genericTypeParameters: (typeReference.GenericTypeParameters ?? Enumerable.Empty<ITypeReference>())
                        .Select(Get)
                        .ToArray()
                    );
            }
        }
    }
}
