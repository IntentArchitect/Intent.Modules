using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
		var invocation = new CSharpInvocationStatement(GetTargetPathExpression());
		// Determine if this model is a method on the class:
		if (TryGetMethodDeclaration(_template, out var method))
        {
			foreach (var parameter in method.Parameters)
			{
				bool optional = parameter.DefaultValue != null;
				var mappedChild = Children.FirstOrDefault(c => c.Model.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
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
        yield return GetSourceStatement(false);
    }

	private bool TryGetMethodDeclaration(ICSharpTemplate typeTemplate, out ICSharpMethodDeclaration method)
	{
        var foundTemplates = _template.GetAllTypeInfo(((IElement)Model).ParentElement.AsTypeReference())
            .Select(x => x.Template).OfType<ICSharpTemplate>().ToList();
        foreach (var foundTemplate in foundTemplates)
        {
            if (foundTemplate?.RootCodeContext?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is ICSharpMethodDeclaration member)
            {
                method = member;
                return true;
            }
        }

        // For backward compatibility and safety reasons (otherwise the above should be sufficient)
		// TODO: Test taking this and running it against all the tests:
        if (typeTemplate is ICSharpFileBuilderTemplate fileBuilderTemplate)
        {
            if (fileBuilderTemplate?.CSharpFile.TypeDeclarations.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is ICSharpMethodDeclaration classMethod)
            {
                method = classMethod;
                return true;
            }
            if (fileBuilderTemplate?.CSharpFile.Interfaces.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out reference) == true && reference is ICSharpMethodDeclaration interfaceMethod)
            {
                method = interfaceMethod;
                return true;
            }
        }

        method = null;
		return false;
	}

}