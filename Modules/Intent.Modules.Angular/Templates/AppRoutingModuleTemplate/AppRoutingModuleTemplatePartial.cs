using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.AppRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AppRoutingModuleTemplate : IntentTypescriptProjectItemTemplateBase<IList<IModuleModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.AppRoutingModuleTemplate";

        public AppRoutingModuleTemplate(IProject project, IList<IModuleModel> model) : base(TemplateId, project, model)
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