using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.OpenCodePlugin
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class OpenCodePluginTemplateRegistration : SingleFileListModelTemplateRegistration<object>
    {
        private readonly IMetadataManager _metadataManager;

        public OpenCodePluginTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => OpenCodePluginTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<object> model)
        {
            return new OpenCodePluginTemplate(outputTarget, model);
        }

        // A single-element list is "generate the file"; empty is "generate nothing" - this
        // template needs no real per-element data, just the conditional itself, so a list of one
        // dummy object stands in for the whole file rather than iterating anything meaningful.
        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<object> GetModels(IApplication application)
        {
            var settings = Intent.Modules.ModuleBuilder.AI.Workflow.Settings.ModuleSettingsExtensions.GetAIWorkflowSettings(application.Settings);
            if (!settings.InstallAgentGateHooks())
            {
                return [];
            }

            var root = application.OutputTargets.FirstOrDefault(t => t.Parent == null)?.Location;
            if (string.IsNullOrEmpty(root) || !Directory.Exists(Path.Combine(root, ".opencode")))
            {
                return [];
            }

            return [new object()];
        }
    }
}