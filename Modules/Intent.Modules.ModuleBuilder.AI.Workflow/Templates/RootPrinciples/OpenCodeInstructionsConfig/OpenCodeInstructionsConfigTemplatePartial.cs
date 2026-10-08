using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.RootPrinciples.OpenCodeInstructionsConfig
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class OpenCodeInstructionsConfigTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.RootPrinciples.OpenCodeInstructionsConfig";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public OpenCodeInstructionsConfigTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string HarnessFolder = ".opencode";
        private const string InstructionsRole = "AI.Context.Instructions";

        /// <summary>
        /// OpenCode loads always-on instructions only from AGENTS.md or from the "instructions" list in
        /// its config - never from a folder by convention. So the instruction files generated into an
        /// AI.Context.Instructions folder under ".opencode" are listed in ".opencode/opencode.json", a
        /// config file OpenCode reads from every ".opencode" folder between the working directory and
        /// the repository root. Declines when ".opencode" has no instructions folder to point at.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == HarnessFolder && InstructionGlobs().Count > 0;
        }

        /// <summary>
        /// One glob per instructions folder under ".opencode", relative to the project root: OpenCode
        /// resolves a relative entry by searching up from its working directory, not from the config
        /// file's own folder.
        /// </summary>
        private IReadOnlyList<string> InstructionGlobs()
        {
            // The project root is the outermost output target - the application's own output folder.
            var projectRootTarget = OutputTarget;
            while (projectRootTarget.Parent != null)
            {
                projectRootTarget = projectRootTarget.Parent;
            }

            var projectRoot = projectRootTarget.Location;
            return ExecutionContext.OutputTargets
                .Where(target => target.HasRole(InstructionsRole) && IsInside(target, OutputTarget))
                .Select(target => Path.GetRelativePath(projectRoot, target.Location).Replace('\\', '/') + "/*.md")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(glob => glob, StringComparer.Ordinal)
                .ToList();
        }

        private static bool IsInside(IOutputTarget target, IOutputTarget ancestor)
        {
            for (var current = target; current != null; current = current.Parent)
            {
                if (current.Id == ancestor.Id)
                {
                    return true;
                }
            }

            return false;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(fileName: "opencode", fileExtension: "json", relativeLocation: "");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Merged, never owned: opencode.json also carries the developer's own providers, MCP servers
            // and permissions. Entries are only ever added; an unparseable file is left as it is.
            var path = Path.Combine(OutputTarget.Location, "opencode.json");
            var outcome = HookConfigMerge.Load(path, out var root, out var existing, out var problem);
            if (outcome == HookConfigMerge.LoadOutcome.Unusable)
            {
                Logging.Log.Warning(
                    $"{TemplateId}: '{path}' was left exactly as it is because {problem}. " +
                    "OpenCode will not load the module-building workflow instructions until the file parses as a JSON object.");
                return existing;
            }

            if (root["$schema"] is null)
            {
                root["$schema"] = "https://opencode.ai/config.json";
            }

            if (root["instructions"] is not JsonArray instructions)
            {
                instructions = new JsonArray();
                root["instructions"] = instructions;
            }

            foreach (var glob in InstructionGlobs())
            {
                if (!instructions.Any(entry => entry is JsonValue value && value.TryGetValue<string>(out var text) && text == glob))
                {
                    instructions.Add(glob);
                }
            }

            return HookConfigMerge.Render(root);
        }
    }
}
