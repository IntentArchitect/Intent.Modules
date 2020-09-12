using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Angular.Templates.App.AppModuleTemplate;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Templates.Shared.Header.HeaderComponentTsTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class HeaderComponentTsTemplate : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Layout.Templates.Shared.Header.HeaderComponentTsTemplate";

        public HeaderComponentTsTemplate(IProject project, object model) : base(TemplateId, project, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypeScriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "header.component",
                relativeLocation: "ClientApp/src/app/shared/header",
                className: "HeaderComponent"
            );
        }

        public override void BeforeTemplateExecution()
        {
            if (File.Exists(GetMetadata().GetFullLocationPathWithFileName()))
            {
                return;
            }

            // New Component:
            Project.Application.EventDispatcher.Publish(AngularComponentCreatedEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularComponentCreatedEvent.ModuleId, "AppModule" },
                    {AngularComponentCreatedEvent.ModelId, TemplateId},
                });
        }
    }
}