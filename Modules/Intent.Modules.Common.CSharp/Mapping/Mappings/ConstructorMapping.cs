using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ConstructorMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly ConstructorMappingOptions _options;

    public ConstructorMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
        _options = new ConstructorMappingOptions();
    }

    public ConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
        _options = new ConstructorMappingOptions();
    }

    public ConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template, ConstructorMappingOptions options) : base(model, template)
    {
        _template = template;
        _options = options ?? new ConstructorMappingOptions();
    }

    public override CSharpStatement GetSourceStatement()
    {
        var typeTemplate = _template.GetTypeInfo(((IElement)Model).ParentElement.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
        // Determine if this model is a constructor on the class:
        if (typeTemplate?.CSharpFile.Classes.First().TryGetReferenceForModel(Model.Id, out var reference) == true && reference is CSharpConstructor ctor) 
        {
            //var i = new CSharpInvocationStatement($"new {reference.Name}").WithoutSemicolon(); (replaced by below)
            //This is to ensure that full typename resolution is taken. Consider exposing the full type via the IHasCSharpName interface.
            var i = new CSharpInvocationStatement($"new {((IntentTemplateBase)_template).GetTypeName(typeTemplate)}").WithoutSemicolon();
            foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
            {
                i.AddArgument(new CSharpArgument(child.GetSourceStatement()), arg =>
                {
                    if (_options.AddArgumentNames)
                    {
                        var index = Children.IndexOf(child);
                        if (index >= ctor.Parameters.Count)
                        {
                            throw new ElementException(Model, "Too many arguments specified in mapping to this constructor");
                        }
                        var argumentName = ctor.Parameters[index];
                        arg.WithName(argumentName.Name);
                    }
                });
            }

            i.WithArgumentsOnNewLines();

            return i;
        }
        // Implicit constructor:
        var init = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
            ? new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}").WithoutSemicolon()
            : new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model)}").WithoutSemicolon();

        foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
        {
            init.AddArgument(new CSharpArgument(child.GetSourceStatement()), arg =>
            {
                if (_options.AddArgumentNames)
                {
                    arg.WithName(child.Model.Name.ToParameterName());
                }
            });
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

public class ConstructorMappingOptions
{
    public bool AddArgumentNames { get; set; } = true;
}