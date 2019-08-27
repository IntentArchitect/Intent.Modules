using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        protected override string ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            var type = typeInfo.Element.GetStereotypeProperty<string>("TypeScript", "Type");

            return !string.IsNullOrWhiteSpace(type)
                ? type
                : typeInfo.Element.Name;
        }

    }
}
