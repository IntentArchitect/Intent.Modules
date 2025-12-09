using Intent.Metadata.Models;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Typescript.Mapping;

public class StaticMethodInvocationMapping : MethodInvocationMapping
{
    private readonly ITypescriptTemplate _template;

    public StaticMethodInvocationMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override TypescriptStatement GetSourceStatement(bool? withNullConditionalOperators = default)
    {
        var invocation = base.GetSourceStatement(withNullConditionalOperators);
        // TODO
        return new TypescriptStatement($"{(invocation.Reference != null
                ? _template.GetTypeName(invocation.Reference.File.Template)
                : _template.GetTypeName(((IElement)Model).ParentElement))} = {invocation}");
    }
}