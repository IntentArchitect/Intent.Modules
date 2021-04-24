using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Templates.InstallationSettingsFile
{
    [IntentManaged(Mode.Merge)]
    partial class InstallationSettingsFileTemplate : IntentTemplateBase<InstallationSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ApplicationTemplate.Builder.Templates.InstallationSettingsFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public InstallationSettingsFileTemplate(IOutputTarget outputTarget, InstallationSettingsModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "settings"
            );
        }
    }
}