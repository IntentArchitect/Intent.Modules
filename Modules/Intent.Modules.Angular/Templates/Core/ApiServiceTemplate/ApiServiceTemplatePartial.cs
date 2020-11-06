using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Modules.Angular.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Core.ApiServiceTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiServiceTemplate
        : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Core.ApiServiceTemplate.ApiServiceTemplate";

        public ApiServiceTemplate(IOutputTarget project, object model) : base(TemplateId, project, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(AngularConfigVariableRequiredEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularConfigVariableRequiredEvent.VariableId, "api_url" },
                    {AngularConfigVariableRequiredEvent.DefaultValueId, "\"https://localhost:{port}/api\"" },
                });
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "api.service",
                relativeLocation: "",
                className: "ApiService"
            );
        }


    }
}