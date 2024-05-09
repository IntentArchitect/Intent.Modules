using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Mapping
{
	public class SelectToListMapping : CSharpMappingBase
	{
		private readonly MappingModel _mappingModel;
		private readonly ICSharpFileBuilderTemplate _template;

		public SelectToListMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
		{
			_mappingModel = model;
			_template = template;
		}
		public override CSharpStatement GetSourceStatement()
		{
			if (Children.Count == 0)
			{
				return $"{GetSourcePathText()}";
			}

			Template.CSharpFile.AddUsing("System.Linq");
			var chain = new CSharpMethodChainStatement($"{GetSourcePathText()}{(Mapping.SourceElement.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
			var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

			var variableName = string.Join("", Model.Name.Pluralize().ToCamelCase());

			var itemMapping = _mappingModel.GetItemMapping();
			itemMapping.SetSourceReplacement(GetSourcePath().Last().Element, variableName);
			itemMapping.SetTargetReplacement(GetTargetPath().Last().Element, null);
			select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(itemMapping.GetSourceStatement()));
			var init = chain
				.AddChainStatement(select)
				.AddChainStatement("ToList()");
			return init;
		}

		public override CSharpStatement GetTargetStatement()
		{
			// TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
			return Model.Name.ToPascalCase();
		}
	}
}
