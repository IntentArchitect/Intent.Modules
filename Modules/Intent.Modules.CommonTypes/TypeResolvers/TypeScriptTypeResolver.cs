using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        protected override string ResolveType(ITypeReference typeInfo, string collectionType = null)
        {
            var type = typeInfo.GetStereotypeProperty<string>("TypeScript", "Type");
            
            return !string.IsNullOrWhiteSpace(type)
                ? type
                : typeInfo.Name;
        }

    }
}
