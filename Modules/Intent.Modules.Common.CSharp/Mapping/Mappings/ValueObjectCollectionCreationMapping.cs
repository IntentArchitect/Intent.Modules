using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Mapping
{
    public class ValueObjectCollectionCreationMapping : CSharpMappingBase
    {
        private readonly MappingModel _mappingModel;
        private readonly ICSharpTemplate _template;

        public ValueObjectCollectionCreationMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
        {
            _mappingModel = model;
            _template = template;
        }

        [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
        public ValueObjectCollectionCreationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : this(model, (ICSharpTemplate)template)
        {
        }

        public override CSharpStatement GetSourceStatement()
        {
            if (Children.Count == 0)
            {
                return $"{GetSourcePathText()}";
            }

            Template.AddUsing("System.Linq");
            var chain = new CSharpMethodChainStatement($"{GetSourcePathText()}{(Mapping.SourceElement.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
            var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

            var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
            SetSourceReplacement(GetSourcePath().Last().Element, variableName);
            SetTargetReplacement(GetTargetPath().Last().Element, null);

            select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(GetConstructorStatement(variableName)));

            var init = chain
                .AddChainStatement(select)
                .AddChainStatement("ToList()");
            return init;
        }

        private CSharpStatement GetConstructorStatement(string variableName)
        {
            var ctor = new ConstructorMapping(_mappingModel, _template);
            ctor.SetSourceReplacement(GetSourcePath().Last().Element, variableName);
            return ctor.GetSourceStatement();
        }

        public override CSharpStatement GetTargetStatement()
        {
            // TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
            return Model.Name.ToPascalCase();
        }

    }
}
