using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilders.Templates.File.FileSingleFileT4
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class FileSingleFileT4Template : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.File.FileSingleFileT4Template";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public FileSingleFileT4Template(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"FileSingleFileT4",
                fileExtension: "txt"
            );
        }
    }
}