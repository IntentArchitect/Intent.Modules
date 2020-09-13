using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Model.ModelTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ModelTemplate : TypeScriptTemplateBase<ModelDefinitionModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Model.ModelTemplate";

        public ModelTemplate(IProject project, ModelDefinitionModel model) : base(TemplateId, project, model)
        {
        }

        public string GetGenericParameters()
        {
            return Model.GenericTypes.Any() ? $"<{string.Join(", ", Model.GenericTypes)}>" : "";
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypeScriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: GetFileName(),
                relativeLocation: $"ClientApp/src/app/{(Model.Module != null ? Model.Module.GetModuleName().ToKebabCase() + "/models" : "models")}",
                className: "${Model.Name}"
            );
        }

        private string GetFileName()
        {
            var modelName = Model.Name.EndsWith("Model") ? Model.Name.Substring(0, Model.Name.Length - "Model".Length) : Model.Name;
            return $"{modelName.ToKebabCase()}.model";
        }
    }
}