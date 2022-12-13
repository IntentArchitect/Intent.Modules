using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Dart.TypeResolvers
{
    public class DartTypeResolver : TypeResolverBase, ITypeResolver
    {
        public DartTypeResolver() : base(new DartTypeResolverContext(CollectionFormatter.Create("{0}[]"), Intent.Modules.Common.TypeResolution.DefaultNullableFormatter.Instance))
        {
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new DartTypeResolverContext(CollectionFormatter.Create("{0}[]"), Intent.Modules.Common.TypeResolution.DefaultNullableFormatter.Instance);
        }

        private class DartTypeResolverContext : TypeResolverContextBase
        {
            public DartTypeResolverContext(
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
                if (typeReference.Element.HasStereotype("Dart"))
                {
                    name = typeReference.Element.GetStereotypeProperty<string>("Dart", "Type");
                }
                else
                {
                    isPrimitive = true;
                    switch (typeReference.Element.Name)
                    {
                        case "bool":
                            name = "boolean";
                            break;
                        case "DateTime":
                            name = "Date";
                            break;
                        case "ByteData":
                        case "double":
                        case "Float":
                        case "Short":
                        case "int":
                        case "Long":
                            name = "number";
                            break;
                        case "Object":
                            name = "any";
                            break;
                        case "String":
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

