using Intent.MetaModel.Common;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        protected override string ResolveType(ITypeReference typeInfo)
        {
            var type = typeInfo.GetStereotypeProperty<string>("TypeScript", "Type");
            
            return !string.IsNullOrWhiteSpace(type)
                ? type
                : typeInfo.Name;
        }

    }
}
