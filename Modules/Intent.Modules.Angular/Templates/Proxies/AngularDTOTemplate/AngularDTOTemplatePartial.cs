using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AngularDTOTemplate : AngularTypescriptProjectItemTemplateBase<ModuleDTOModel> 
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Proxies.AngularDTOTemplate";

        public AngularDTOTemplate(IProject project, ModuleDTOModel model) : base(TemplateId, project, model, TypescriptTemplateMode.AlwaysRecreateFromTemplate)
        {
            AddTypeSource(TypescriptTypeSource.InProject(Project, TemplateId));
        }

        public string GenericTypes => Model.GenericTypes.Any() ? $"<{ string.Join(", ", Model.GenericTypes) }>" : "";

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            //var moduleTemplate = Project.FindTemplateInstance<AngularModuleTemplate.AngularModuleTemplate>(AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"ClientApp/src/app/{Model.Module.GetModuleName().ToKebabCase()}/models",
                className: "${Model.Name}"
            );
        }
    }
}