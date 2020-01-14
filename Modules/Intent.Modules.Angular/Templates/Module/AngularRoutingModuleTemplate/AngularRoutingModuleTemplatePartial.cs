using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Module.AngularRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularRoutingModuleTemplate : IntentTypescriptProjectItemTemplateBase<IModuleModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Module.AngularRoutingModuleTemplate";

        public AngularRoutingModuleTemplate(IProject project, IModuleModel model) : base(TemplateId, project, model)
        {
        }

        public string ModuleName => Model.GetModuleName() + "Routing";


        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ModuleName.ToAngularFileName()}.module",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client/src/app/{ Model.GetModuleName().ToAngularFileName() }",
                className: "${ModuleName}"
            );
        }


        private string GetPath(IComponentModel component)
        {
            return GetTemplateClassName(AngularComponentTsTemplate.TemplateId, component).Replace("Component", "").ToAngularFileName();
        }

        private string GetClassName(IComponentModel component)
        {
            return GetTemplateClassName(AngularComponentTsTemplate.TemplateId, component);
        }
    }
}