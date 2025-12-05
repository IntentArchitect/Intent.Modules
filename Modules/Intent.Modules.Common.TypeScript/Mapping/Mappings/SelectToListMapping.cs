using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Angular.Mapping
{
    public class SelectToListMapping : TypescriptMappingBase
    {
        private readonly MappingModel _mappingModel;
        private readonly ITypescriptTemplate _template;

        [Obsolete("Use overload with ICSharpTemplate template")]
        public SelectToListMapping(MappingModel model, ITypescriptFileBuilderTemplate template) : this(model, (ITypescriptTemplate)template) { }
        public SelectToListMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
        {
            _mappingModel = model;
            _template = template;
        }

        public override TypescriptStatement GetSourceStatement(bool? withNullConditionalOperators = default)
        {
            if (Children.Count == 0)
            {
                return $"{GetSourcePathText()}";
            }

            var chain = new TypescriptStatement($"{GetSourcePathText()}{(Mapping.SourceElement.TypeReference.IsNullable ? "?" : "")}");
            var map = new TypescriptStatement($".map");

            var variableName = GetVariableNameForSelect();

            var itemMapping = _mappingModel.GetCollectionItemMapping();
            itemMapping.Parent = Parent;

            itemMapping.SetSourceReplacement(GetSourcePath().Last().Element, variableName);
            itemMapping.SetTargetReplacement(GetTargetPath().Last().Element, null);

            var sourceStatement = itemMapping.GetSourceStatement();

            var init = new TypescriptStatement(@$"{chain}{map}({variableName} => ({sourceStatement}))");
            return init;
        }

        private string GetVariableNameForSelect()
        {
            var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
            if (string.IsNullOrEmpty(variableName))
            {
                variableName = Char.ToLower(Model.Name[0]).ToString();
            }

            return variableName;
        }

        public override TypescriptStatement GetTargetStatement()
        {
            // TODO: Please revisit, this only writing out the property name and doesn't allow for accessor variables
            return Model.Name.ToPascalCase();
        }
    }
}
