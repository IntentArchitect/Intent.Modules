using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
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

        /// <summary>
        /// Suppressed the same way as the other hook registrations. An empty list from GetModels does
        /// not do it: SingleFileListModelTemplateRegistration.Register registers the template whatever
        /// the list holds, so the plugin used to be generated with the switch off.
        /// </summary>
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!AgentGateSwitch.IsOn(application))
            {
                return;
            }

            base.Register(registry, application);
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<object> model)
        {
            return new OpenCodePluginTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<object> GetModels(IApplication application)
        {
            // The template needs no per-element data, so one dummy object stands in for the file. The
            // switch is checked in Register, not here: an empty list still generates the file.
            //
            // Kept inside the body deliberately: this member's signature is Mode.Fully, so a comment
            // above it is stripped on every regeneration. Body = Mode.Ignore is what protects it.
            //
            // No disk probing for ".opencode" - the anchor decides. This template is instantiated
            // once per AI.Context anchor and CanRunTemplate declines anywhere but ".opencode".
            return [new object()];
        }
    }
}