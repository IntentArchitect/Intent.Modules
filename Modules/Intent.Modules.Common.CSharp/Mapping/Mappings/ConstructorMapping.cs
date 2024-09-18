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
    private readonly ICSharpTemplate _template;
    private readonly ConstructorMappingOptions _options;
    private readonly MappingModel _mappingMappingModel;

    public ConstructorMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
        _mappingMappingModel = model;
        _template = template;
        _options = new ConstructorMappingOptions();
    }

    public ConstructorMapping(MappingModel model, ICSharpTemplate template, ConstructorMappingOptions options) : base(model, template)
    {
        _mappingMappingModel = model;
        _template = template;
        _options = options ?? new ConstructorMappingOptions();
    }

    [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
    public ConstructorMapping(MappingModel model, ICSharpFileBuilderTemplate template) : this(model, (ICSharpTemplate)template) { }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
        //Try find the CSharp constructor so we know what parameters are expected and in what order
        if (TryFindModelConstructor(out var ctor)) 
        {
			var inv = new CSharpInvocationStatement($"new { ctor.Name }").WithoutSemicolon();

			foreach (var parameter in ctor.Parameters)
            {
                var optional = parameter.DefaultValue != null;
				var child = GetAllChildren().FirstOrDefault(c => c.Model.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
				if (child != null)
				{
					inv.AddArgument(new CSharpArgument(child.GetSourceStatement()), arg =>
					{
						if (_options.AddArgumentNames)
						{
							arg.WithName(parameter.Name);
						}
					});
				}
				else if (!optional)
                {
                    inv.AddArgument(new CSharpArgument("default"), arg => 
                    {
						if (_options.AddArgumentNames)
                        {
							arg.WithName(parameter.Name);
						}
					});
    			}
			}

            inv.WithArgumentsOnNewLines();

            return inv;
        }

        //This is not ideal and a best effort to realize your mapping

        // Implicit constructor (this assumes a 1->1 mapping in the exact order):
        var init = !((IElement)Model).ChildElements.Any() && Model.TypeReference != null
            ? new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}").WithoutSemicolon()
            : new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model)}").WithoutSemicolon();

		//foreach (var child in GetAllChildren().Where(c => c is not MapChildrenMapping))
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
        yield return new CSharpAssignmentStatement(GetTargetStatement(), GetNullableAwareInstantiation(Model, Children, GetSourceStatement()));
    }

	private bool TryFindModelConstructor(out CSharpConstructor constructor)
	{
		//for Elements mapped to a metadata constructor, use the constructors meta data class to find its template and get the CSharp constructor
		constructor = null;
		// Model assumes to be the Constructor element and it needs to access the owner of this Constructor to fetch the Template
		var foundTemplates = _template.GetAllTypeInfo(((IElement)Model).ParentElement.AsTypeReference())
            .Select(x => x.Template).OfType<ICSharpFileBuilderTemplate>().ToList();
        foreach (var foundTemplate in foundTemplates)
        {
            // Determine if this model is a constructor on the class:
            if (foundTemplate?.CSharpFile.TypeDeclarations.FirstOrDefault()?.TryGetReferenceForModel(Model.Id, out var reference) == true && reference is CSharpConstructor ctor)
            {
                _template.AddUsing(foundTemplate.Namespace);
                constructor = ctor;
                return true;

            }
        }


		//This is for implicit constructors.. use the model to find the template and find it's public constructor with the most arguments 
		if (!((IElement)Model).ChildElements.Any() && Model.TypeReference != null)
		{
			var modelTemplate = _template.GetTypeInfo(((IElement)Model).TypeReference.Element?.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
			var mainType = modelTemplate?.CSharpFile.TypeDeclarations.FirstOrDefault(t => t.Constructors.Count > 0);
			if (mainType == null) 
				return false;
			constructor = mainType.Constructors
					.Where(c => c.AccessModifier == "public ")
					.OrderByDescending(c => c.Parameters.Count)
					.FirstOrDefault();
			if (constructor != null)
			{
				_template.AddUsing(modelTemplate.Namespace);
			}
			return constructor != null;
		}

		return false;
	}


}

public class ConstructorMappingOptions
{
    public bool AddArgumentNames { get; set; } = true;
}