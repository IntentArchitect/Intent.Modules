using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates.Core.CoreModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.App.AppModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AppModuleTemplate : AngularTypescriptProjectItemTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.App.AppModuleTemplate";

        public AppModuleTemplate(IProject project, object model) : base(TemplateId, project, model, TypescriptTemplateMode.UpdateFile)
        {
        }

        public string AppRoutingModuleClassName => GetTemplateClassName(AppRoutingModuleTemplate.AppRoutingModuleTemplate.TemplateId);
        public string CoreModule => GetTemplateClassName(CoreModuleTemplate.TemplateId);

        protected override void ApplyFileChanges(TypescriptFile file)
        {
            var moduleClass = file.ClassDeclarations().First();

            var moduleDecorator = moduleClass.Decorators().FirstOrDefault(x => x.Name == "NgModule")?.ToNgModule();

            moduleDecorator?.AddImportIfNotExists(CoreModule);
            moduleDecorator?.AddImportIfNotExists(AppRoutingModuleClassName);
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