using Intent.MetaModel.Common;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.TypeResolvers;

namespace Intent.Modules.CommonTypes.TypeResolvers
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
