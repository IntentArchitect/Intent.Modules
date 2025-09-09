using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class StaticMethodInvocationMapping : MethodInvocationMapping
{
    private readonly ICSharpTemplate _template;

    public StaticMethodInvocationMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override CSharpStatement GetSourceStatement(bool? withNullConditionalOperators = default)
    {
        var invocation = base.GetSourceStatement(withNullConditionalOperators).WithoutSemicolon();
        return new CSharpAccessMemberStatement(invocation.Reference != null
                ? _template.GetTypeName(invocation.Reference.File.Template)
                : _template.GetTypeName(((IElement)Model).ParentElement),
            invocation);
    }
}