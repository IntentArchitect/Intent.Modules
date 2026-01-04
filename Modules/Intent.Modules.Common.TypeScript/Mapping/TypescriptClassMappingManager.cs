using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Typescript.Mapping;

public class TypescriptClassMappingManager : MappingManagerBase
{
    public TypescriptClassMappingManager(ITypescriptFileBuilderTemplate template) : base(template)
    {
    }

    public TypescriptClassMappingManager(ITypescriptTemplate template) : base(template)
    {
    }
}