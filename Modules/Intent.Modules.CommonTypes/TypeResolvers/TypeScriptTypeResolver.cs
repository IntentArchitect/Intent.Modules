using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        public override string DefaultCollectionFormat { get; set; } = "{0}[]";

        protected override string ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            var type = typeInfo.Element.GetStereotypeProperty<string>("TypeScript", "Type");

            var result = !string.IsNullOrWhiteSpace(type)
                ? type
                : typeInfo.Element.Name;

            if (typeInfo.IsCollection)
            {
                result = string.Format(collectionFormat ?? DefaultCollectionFormat, result);
            }

            return result;
        }

    }
}
