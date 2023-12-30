using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class MethodInvocationMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public MethodInvocationMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public MethodInvocationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override CSharpStatement GetSourceStatement()
    {
        var invocation = new CSharpInvocationStatement(GetTargetPathExpression());

        foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
        {
            invocation.AddArgument(child.GetSourceStatement());
        }

        if (Children.Count > 3)
        {
            invocation.WithArgumentsOnNewLines();
        }

        return invocation;
    }

    //public override CSharpStatement GetTargetStatement()
    //{
    //    return GetSourcePathText(Children.First(x => x.Mapping != null).Mapping.TargetPath.SkipLast(1).ToList());
    //}

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return GetSourceStatement();
    }
}