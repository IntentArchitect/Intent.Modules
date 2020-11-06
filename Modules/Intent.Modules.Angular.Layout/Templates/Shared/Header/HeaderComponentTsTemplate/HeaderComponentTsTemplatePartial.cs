using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Angular.Templates.App.AppModuleTemplate;
using Intent.Modules.Angular.Templates.Shared.IntentDecoratorsTemplate;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Angular.Layout.Api;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Templates.Shared.Header.HeaderComponentTsTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class HeaderComponentTsTemplate : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Layout.Shared.Header.HeaderComponentTsTemplate.HeaderComponentTsTemplate";

        public HeaderComponentTsTemplate(IOutputTarget project, object model) : base(TemplateId, project, model)
        {
            AddTemplateDependency(IntentDecoratorsTemplate.TemplateId);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "header.component",
                relativeLocation: "",
                className: "HeaderComponent"
            );
        }

        public override void BeforeTemplateExecution()
        {
            if (File.Exists(GetMetadata().GetFilePath()))
            {
                return;
            }

            // New Component:
            ExecutionContext.EventDispatcher.Publish(AngularComponentCreatedEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularComponentCreatedEvent.ModuleId, "AppModule" },
                    {AngularComponentCreatedEvent.ModelId, TemplateId},
                });

            ExecutionContext.EventDispatcher.Publish(AngularImportDependencyRequiredEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularImportDependencyRequiredEvent.ModuleId, "AppModule" },
                    {AngularImportDependencyRequiredEvent.Dependency, "CollapseModule.forRoot()"},
                    {AngularImportDependencyRequiredEvent.Import, "import { CollapseModule } from 'ngx-bootstrap/collapse';"},
                });
        }
    }
}