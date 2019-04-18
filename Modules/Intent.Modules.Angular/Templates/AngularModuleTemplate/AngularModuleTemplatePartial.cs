using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Templates.Component.AngularComponentCssTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.AngularModuleTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularModuleTemplate : IntentProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Angular.AngularModuleTemplate";

        public AngularModuleTemplate(IProject project, IClass model) : base(TemplateId, project, model)
        {

        }

        public string ModuleName
        {
            get
            {
                if (Model.Name.EndsWith("Module", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Model.Name.Substring(0, Model.Name.Length - "Module".Length);
                }
                return Model.Name;
            }
        }

        public override string RunTemplate()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var editor = new AngularModuleEditor(source);
            foreach (var componentModel in Model.ChildClasses)
            {
                editor.AddImportIfNotExists($"{GetComponentName(componentModel)}Component", $"./{ GetComponentName(componentModel).ToAngularFileName() }/{ GetComponentName(componentModel).ToAngularFileName() }.component");
                editor.AddDeclarationIfNotExists($"{GetComponentName(componentModel)}Component");
            }

            return editor.GetSource();
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : TransformText();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ModuleName.ToAngularFileName()}.module",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client\\src\\app\\{ ModuleName.ToAngularFileName() }"
            );
        }

        private string GetComponentName(IClass componentModel)
        {
            var componentTemplate = Project.FindTemplateInstance<Component.AngularComponentTsTemplate.AngularComponentTsTemplate>(Component.AngularComponentTsTemplate.AngularComponentTsTemplate.TemplateId, componentModel);
            return componentTemplate.ComponentName;
        }
    }
}