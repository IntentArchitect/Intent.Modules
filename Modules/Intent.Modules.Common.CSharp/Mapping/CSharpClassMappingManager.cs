using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class CSharpClassMappingManager : MappingManagerBase
{
    public CSharpClassMappingManager(ICSharpFileBuilderTemplate template) : base(template)
    {
    }
}