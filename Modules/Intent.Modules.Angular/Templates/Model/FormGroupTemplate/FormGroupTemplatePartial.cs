using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Model.FormGroupTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class FormGroupTemplate : TypeScriptTemplateBase<FormGroupDefinitionModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Model.FormGroupTemplate.FormGroupTemplate";

        public FormGroupTemplate(IOutputTarget project, FormGroupDefinitionModel model) : base(TemplateId, project, model)
        {
            AddTypeSource(FormGroupTemplate.TemplateId);
            AddTypeSource(AngularDTOTemplate.TemplateId);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name.ToKebabCase()}.model",
                relativeLocation: $"{(Model.Module != null ? Model.Module.GetModuleName().ToKebabCase() + "/models" : "models")}",
                className: "${Model.Name}"
            );
        }

        public override string RunTemplate()
        {
            ;
            try
            {
                return base.RunTemplate();
            }
            catch (Exception e)
            {
                Logging.Log.Failure(e);
                return TransformText();
            }
        }

        private string GetFileName()
        {
            var modelName = Model.Name.EndsWith("Model") ? Model.Name.Substring(0, Model.Name.Length - "Model".Length) : Model.Name;
            return $"{modelName.ToKebabCase()}.model";
        }

        public string GetPath(IEnumerable<IElementMappingPathTarget> path)
        {
            return string.Join(".", path.Select(x => x.Name.ToCamelCase()));
        }
    }
}