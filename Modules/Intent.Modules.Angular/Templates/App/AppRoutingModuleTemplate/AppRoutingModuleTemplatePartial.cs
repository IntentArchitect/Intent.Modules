using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using System;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.App.AppRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AppRoutingModuleTemplate : TypeScriptTemplateBase<IList<ModuleModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.App.AppRoutingModuleTemplate";

        public AppRoutingModuleTemplate(IProject project, IList<ModuleModel> model) : base(TemplateId, project, model, TypescriptTemplateMode.AlwaysRecreateFromTemplate)
        {
        }

        public IEnumerable<AngularModuleTemplate> ModuleTemplates
        {
            get
            {
                return Model.Select(x => FindTemplate<AngularModuleTemplate>(TemplateDependency.OnModel(AngularModuleTemplate.TemplateId, x)));
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"app-routing.module",
                fileExtension: "ts",
                defaultLocationInProject: $"Client/src/app",
                className: "AppRoutingModule"
            );
        }


    }
}