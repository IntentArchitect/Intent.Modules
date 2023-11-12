using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ConstructorMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ConstructorMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public ConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override CSharpStatement GetSourceStatement()
    {
        var typeTemplate = _template.GetTypeInfo(((IElement)Model).ParentElement.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
        if (typeTemplate?.CSharpFile.Classes.First().TryGetReferenceForModel(Model.Id, out _) == true)
        {
            var i = new CSharpInvocationStatement($"new {_template.GetTypeName(((IElement)Model).ParentElement)}").WithoutSemicolon();
            foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
            {
                i.AddArgument(child.GetSourceStatement());
            }

            i.WithArgumentsOnNewLines();

            return i;
        }
        var init = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
            ? new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}").WithoutSemicolon()
            : new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model)}").WithoutSemicolon();

        foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
        {
            init.AddArgument(child.GetSourceStatement());
        }

        if (Children.Count > 3)
        {
            init.WithArgumentsOnNewLines();
        }

        return init;
    }

    public override CSharpStatement GetTargetStatement()
    {
        return GetPathText(Children.First(x => x.Mapping != null).Mapping.TargetPath.SkipLast(1).ToList(), _targetReplacements);
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
    }
}