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
    private readonly MappingModel _mappingMappingModel;

    public ConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _mappingMappingModel = model;
        _template = template;
        _options = new ConstructorMappingOptions();
    }

    public ConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template, ConstructorMappingOptions options) : base(model, template)
    {
        _mappingMappingModel = model;
        _template = template;
        _options = options ?? new ConstructorMappingOptions();
    }

    public override CSharpStatement GetSourceStatement()
    {
        // Model assumes to be the Constructor element and it needs to access the owner of this Constructor to fetch the Template
        var typeTemplate = _template.GetTypeInfo(((IElement)Model).ParentElement.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
        // Determine if this model is a constructor on the class:
        if (typeTemplate?.CSharpFile.Classes.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is CSharpConstructor ctor) 
        {
            //var concreteClassModel = (IElement)_mappingMappingModel.GetParent(x => x.Parent == null || x.Mapping != null)?.Model 
            //                         ?? ((IElement)Model).ParentElement;

            //var i = new CSharpInvocationStatement($"new {reference.Name}").WithoutSemicolon(); (replaced by below)
            //This is to ensure that full typename resolution is taken. Consider exposing the full type via the IHasCSharpName interface.
            var i = new CSharpInvocationStatement($"new {_template.GetTypeName(((IElement)Model).ParentElement)}").WithoutSemicolon();

            foreach (var parameter in ctor.Parameters)
            {
                bool optional = parameter.DefaultValue != null;
				var child = Children.FirstOrDefault(c => c.Model.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
				if ( child != null)
                {
					i.AddArgument(new CSharpArgument(child.GetSourceStatement()), arg =>
					{
						if (_options.AddArgumentNames)
						{
							arg.WithName(parameter.Name);
						}
					});
				}
				else if (!optional)
                {
                    i.AddArgument(new CSharpArgument("default"), arg => 
                    {
						if (_options.AddArgumentNames)
                        {
							arg.WithName(parameter.Name);
						}

					});
    			}
			}

            i.WithArgumentsOnNewLines();

            return i;
        }
        // Implicit constructor (this assumes a 1->1 mapping in the exact order):
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

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
    }
}

public class ConstructorMappingOptions
{
    public bool AddArgumentNames { get; set; } = true;
}