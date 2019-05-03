using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularServiceProxyTemplate : IntentTypescriptProjectItemTemplateBase<IServiceProxyModel>
    {
        public const string TemplateId = "Angular.AngularServiceProxyTemplate";

        public AngularServiceProxyTemplate(IProject project, IServiceProxyModel model) : base(TemplateId, project, model)
        {
        }

        public override string RunTemplate()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var editor = new AngularModuleEditor(source);
            foreach (var componentModel in Model.Operations)
            {
                //editor.AddImportIfNotExists($"{GetComponentName(componentModel)}Component", $"./{ GetComponentName(componentModel).ToAngularFileName() }/{ GetComponentName(componentModel).ToAngularFileName() }.component");
                //editor.AddDeclarationIfNotExists($"{GetComponentName(componentModel)}Component");
            }

            return editor.GetSource();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            var moduleTemplate = Project.FindTemplateInstance<AngularModuleTemplate.AngularModuleTemplate>(AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name.ToAngularFileName()}.service",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client\\src\\app\\{moduleTemplate.ModuleName.ToAngularFileName()}",
                className: "${Model.Name}ServiceProxy"
            );
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : TransformText();
        }
    }
}