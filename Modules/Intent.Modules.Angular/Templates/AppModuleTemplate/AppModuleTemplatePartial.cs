using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates.Core.CoreModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.AppModuleTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AppModuleTemplate : IntentTypescriptProjectItemTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.AppModuleTemplate";

        public AppModuleTemplate(IProject project, object model) : base(TemplateId, project, model)
        {
        }

        public string AppRoutingModuleClassName => GetTemplateClassName(AppRoutingModuleTemplate.AppRoutingModuleTemplate.TemplateId);
        public string CoreModule => GetTemplateClassName(CoreModuleTemplate.TemplateId);

        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var file = new TypescriptFile(source);
            var moduleClass = file.ClassDeclarations().First();

            var moduleDecorator = moduleClass.Decorators().FirstOrDefault(x => x.Name == "NgModule");

            moduleDecorator?.AddImportIfNotExists(CoreModule);
            moduleDecorator?.AddImportIfNotExists(AppRoutingModuleClassName);

            foreach (var templateDependency in GetTemplateDependencies())
            {
                var template = Project.FindTemplateInstance<ITemplate>(templateDependency);
                file.AddImportIfNotExists(((IHasClassDetails)template).ClassName, GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(template.GetMetadata().GetRelativeFilePathWithFileName()));
            }

            return file.GetSource();
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : base.RunTemplate();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"app.module",
                fileExtension: "ts",
                defaultLocationInProject: $"Client/src/app",
                className: "AppModule"
            );
        }


    }
}