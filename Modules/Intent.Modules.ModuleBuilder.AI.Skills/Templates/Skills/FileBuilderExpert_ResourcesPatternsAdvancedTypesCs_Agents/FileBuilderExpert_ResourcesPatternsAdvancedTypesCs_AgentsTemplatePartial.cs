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
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.FileBuilderExpert_ResourcesPatternsAdvancedTypesCs_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class FileBuilderExpert_ResourcesPatternsAdvancedTypesCs_AgentsTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.FileBuilderExpert_ResourcesPatternsAdvancedTypesCs_Agents";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public FileBuilderExpert_ResourcesPatternsAdvancedTypesCs_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"advanced-types",
                fileExtension: "cs",
                relativeLocation: "file-builder-expert/resources/patterns"
            );
        }

    }
}
