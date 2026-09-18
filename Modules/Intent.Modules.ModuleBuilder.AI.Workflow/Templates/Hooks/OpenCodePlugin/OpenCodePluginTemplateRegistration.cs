using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
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

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<object> GetModels(IApplication application)
        {
            // A single-element list is "generate the file"; empty is "generate nothing" - this
            // template needs no real per-element data, just the conditional itself, so a list of one
            // dummy object stands in for the whole file rather than iterating anything meaningful.
            //
            // Kept inside the body deliberately: this member's signature is Mode.Fully, so a comment
            // above it is stripped on every regeneration. Body = Mode.Ignore is what protects it.
            var settings = Intent.Modules.ModuleBuilder.AI.Workflow.Settings.ModuleSettingsExtensions.GetAIWorkflowSettings(application.Settings);
            if (!settings.InstallAgentGateHooks())
            {
                return [];
            }

            // No disk probing for ".opencode" - the anchor decides. This template is instantiated
            // once per AI.Context anchor and CanRunTemplate declines anywhere but ".opencode".
            return [new object()];
        }
    }
}