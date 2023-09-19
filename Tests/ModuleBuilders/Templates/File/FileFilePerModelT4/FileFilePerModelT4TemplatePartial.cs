using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilders.Templates.File.FileFilePerModelT4
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class FileFilePerModelT4Template : IntentTemplateBase<ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.File.FileFilePerModelT4Template";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public FileFilePerModelT4Template(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "txt"
            );
        }
    }
}