using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public class MethodInvocationMapping : CSharpMappingBase
{
    private readonly ICSharpTemplate _template;

    public MethodInvocationMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public MethodInvocationMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
        _template = template;
    }

    [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
    public MethodInvocationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : this(model, (ICSharpTemplate)template)
    {
    }

    public override CSharpStatement GetSourceStatement()
    {
        var invocation = new CSharpInvocationStatement(GetTargetPathExpression());

		var typeTemplate = _template.GetTypeInfo(((IElement)Model).ParentElement.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
		// Determine if this model is a method on the class:
		if (typeTemplate?.CSharpFile.Classes.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is CSharpClassMethod method)
        {
			foreach (var parameter in method.Parameters)
			{
				bool optional = parameter.DefaultValue != null;
				var mappedChild = Children.FirstOrDefault(c => string.Compare(c.Mapping.TargetElement.Name, parameter.Name, true) == 0);
				if (mappedChild != null)
				{
					invocation.AddArgument(mappedChild.GetSourceStatement());
				}
				else if (!optional)
				{
					invocation.AddArgument("default");
				}
			}
			if (method.Parameters.Count() > 3)
			{
				invocation.WithArgumentsOnNewLines();
			}
		}
		else
        {
			foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
			{
				invocation.AddArgument(child.GetSourceStatement());
			}
			if (Children.Count > 3)
			{
				invocation.WithArgumentsOnNewLines();
			}

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