using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.ClaudeSettings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ClaudeSettingsTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.ClaudeSettings";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ClaudeSettingsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string SettingsFileName = "settings.json";

        /// <summary>
        /// ".claude/settings.json" also carries the developer's permissions, environment and
        /// unrelated hooks, so it is merged rather than generated - see <see cref="HookConfigMerge"/>
        /// for the rules and <see cref="HarnessHookSets.MergeClaudeSettings"/> for what is registered.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == ".claude";
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(fileName: "settings", fileExtension: "json", relativeLocation: "");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return MergedHookFile.Transform(
                Path.Combine(OutputTarget.Location, SettingsFileName), TemplateId, "Claude Code", HarnessHookSets.MergeClaudeSettings);
        }
    }
}
