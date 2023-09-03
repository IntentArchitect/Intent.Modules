using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ImplicitConstructorMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ImplicitConstructorMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public ImplicitConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override CSharpStatement GetFromStatement()
    {
        var typeTemplate = _template.GetTypeInfo(((IElement)Model).ParentElement.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
        if (typeTemplate?.CSharpFile.GetReferenceForModel(Model.Id) is CSharpConstructor)
        {
            var i = new CSharpInvocationStatement($"new {_template.GetTypeName(((IElement)Model).ParentElement)}").WithoutSemicolon();
            foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
            {
                i.AddArgument(child.GetFromStatement());
            }

            i.WithArgumentsOnNewLines();

            return i;
        }
        var init = Model.TypeReference != null
            ? new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}").WithoutSemicolon()
            : new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model)}").WithoutSemicolon();

        foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
        {
            init.AddArgument(child.GetFromStatement());
        }

        return init;
    }

    public override CSharpStatement GetToStatement()
    {
        return GetPathText(Children.First(x => x.Mapping != null).Mapping.ToPath.SkipLast(1).ToList(), _toReplacements);
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return new CSharpAssignmentStatement(GetToStatement(), GetFromStatement());
    }
}