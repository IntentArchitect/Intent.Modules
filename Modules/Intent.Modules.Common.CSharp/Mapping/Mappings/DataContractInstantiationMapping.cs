using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class DataContractInstantiationMapping : CSharpMappingBase
{
    private readonly MappingModel _mappingModel;
    private readonly ICSharpFileBuilderTemplate _template;

    public DataContractInstantiationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _mappingModel = model;
        _template = template;
    }
    
    public override CSharpStatement GetSourceStatement()
    {
        if (Model.TypeReference == null)
        {
            SetTargetReplacement(Model, null);
            return GetConstructorStatement(null);
        }
        
        
        if (Children.Count == 0)
        {
            return $"{GetSourcePathText()}";
        }

        if (Model.TypeReference.IsCollection)
        {
            Template.CSharpFile.AddUsing("System.Linq");
            var chain = new CSharpMethodChainStatement($"{GetSourcePathText()}{(Mapping.SourceElement.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
            var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

            var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
            if (string.IsNullOrEmpty(variableName))
            {
                variableName = Char.ToLower(Model.Name[0]).ToString();
            }
            SetSourceReplacement(GetSourcePath().Last().Element, variableName);
            SetTargetReplacement(GetTargetPath().Last().Element, null);

            select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(GetConstructorStatement(variableName)));

            var init = chain
                .AddChainStatement(select)
                .AddChainStatement("ToList()");
            return init;
        }
        
        if (Mapping != null)
        {
            return GetSourcePathText();
        }
        else
        {
            // TODO: add ternary check to mappings for when the source path could be nullable.
            SetTargetReplacement(GetTargetPath().Last().Element, null);
            return GetConstructorStatement(null);
        }
    }

    private CSharpStatement GetConstructorStatement(string variableName)
    {
        var ctor = new ConstructorMapping(_mappingModel, _template);
        if (!string.IsNullOrEmpty(variableName))
        {
            ctor.SetSourceReplacement(GetSourcePath().Last().Element, variableName);
        }
        return ctor.GetSourceStatement();
    }
}