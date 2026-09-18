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
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.HooksJson
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HooksJsonTemplateRegistration : FilePerModelTemplateRegistration<HarnessFolderModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HooksJsonTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => HooksJsonTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, HarnessFolderModel model)
        {
            return new HooksJsonTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<HarnessFolderModel> GetModels(IApplication application)
        {
            // Exactly one model, not one per harness. Multiplicity comes from the AI.Context anchors -
            // this template is instantiated once per anchor, and each instance decides from the folder
            // it landed in whether it has anything to emit. Probing the disk for harness folders here
            // would double-count against that, and was how the same file ended up being generated from
            // three anchors at once.
            //
            // Kept inside the body deliberately: this member's signature is Mode.Fully, so a comment
            // above it is stripped on every regeneration. Body = Mode.Ignore is what protects it.
            var settings = Intent.Modules.ModuleBuilder.AI.Workflow.Settings.ModuleSettingsExtensions.GetAIWorkflowSettings(application.Settings);
            if (!settings.InstallAgentGateHooks())
            {
                yield break;
            }

            yield return new HarnessFolderModel();
        }
    }
}