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

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Core.ApiServiceTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class ApiServiceTemplate : AngularTypescriptProjectItemTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Core.ApiServiceTemplate";

        public ApiServiceTemplate(IProject project, object model) : base(TemplateId, project, model, TypescriptTemplateMode.AlwaysRecreateFromTemplate)
        {
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(AngularConfigVariableRequiredEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularConfigVariableRequiredEvent.VariableId, "api_url" },
                    {AngularConfigVariableRequiredEvent.DefaultValueId, "\"http://localhost:{port}/api\"" },
                });
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "api.service",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: "Client/src/app/core",
                className: "ApiService"
            );
        }


    }
}