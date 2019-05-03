using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularDTOTemplate : IntentTypescriptProjectItemTemplateBase<IModuleDTOModel>
    {
        public const string TemplateId = "Angular.AngularDTOTemplate";

        public AngularDTOTemplate(IProject project, IModuleDTOModel model) : base(TemplateId, project, model)
        {
        }

        public string GenericTypes => Model.GenericTypes.Any() ? $"<{ string.Join(", ", Model.GenericTypes) }>" : "";

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            var moduleTemplate = Project.FindTemplateInstance<AngularModuleTemplate.AngularModuleTemplate>(AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client\\src\\app\\{moduleTemplate.ModuleName.ToAngularFileName()}\\models",
                className: "${Model.Name}"
            );
        }
    }
}