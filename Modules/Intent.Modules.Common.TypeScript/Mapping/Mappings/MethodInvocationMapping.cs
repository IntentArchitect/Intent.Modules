using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Intent.Modules.Common.Angular.Mapping;

public class MethodInvocationMapping : TypescriptMappingBase
{
    private readonly ITypescriptTemplate _template;

    public MethodInvocationMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ITypescriptTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public MethodInvocationMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
    {
        _template = template;
    }

    [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
    public MethodInvocationMapping(MappingModel model, ITypescriptFileBuilderTemplate template) : this(model, (ITypescriptTemplate)template)
    {
    }

    public override TypescriptStatement GetSourceStatement(bool? withNullConditionalOperators = default)
    {
        // Determine if this model is a method on the class:
        if (TryGetMethodDeclaration(_template, out var method))
        {
            var invocationParameters = new List<string>();
            foreach (var parameter in method.Parameters)
            {
                bool optional = parameter.DefaultValue != null;
                var mappedChild = Children.FirstOrDefault(c => c.Model.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
                if (mappedChild != null)
                {
                    invocationParameters.Add(mappedChild.GetSourceStatement().GetText(""));
                }
                else if (!optional)
                {
                    invocationParameters.Add("''");
                }
            }

            return $"{GetTargetPathExpression()}({string.Join(',', invocationParameters)})";
        }
        else
        {
            var invocationParameters = new List<string>();
            var invocation = GetTargetPathExpression();
            foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
            {
                invocationParameters.Add(child.GetSourceStatement().GetText(""));
            }

            return $"{GetTargetPathExpression()}({string.Join(',', invocationParameters)})";
        }
    }

    public override IEnumerable<TypescriptStatement> GetMappingStatements()
    {
        yield return GetSourceStatement(false);
    }

    private bool TryGetMethodDeclaration(ITypescriptTemplate typeTemplate, out TypescriptMethod method)
    {
        var foundTemplates = _template.GetAllTypeInfo(((IElement)Model).ParentElement.AsTypeReference())
            .Select(x => x.Template).OfType<ITypescriptTemplate>().ToList();
        foreach (var foundTemplate in foundTemplates)
        {
            if (foundTemplate?.RootCodeContext?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is TypescriptMethod member)
            {
                method = member;
                return true;
            }
        }

        // For backward compatibility and safety reasons (otherwise the above should be sufficient)
        // TODO: Test taking this and running it against all the tests:
        if (typeTemplate is ITypescriptFileBuilderTemplate fileBuilderTemplate)
        {
            if (fileBuilderTemplate?.TypescriptFile.Classes.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is TypescriptMethod classMethod)
            {
                method = classMethod;
                return true;
            }
            if (fileBuilderTemplate?.TypescriptFile.Classes.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out reference) == true && reference is TypescriptMethod interfaceMethod)
            {
                method = interfaceMethod;
                return true;
            }
        }

        method = null;
        return false;
    }

}