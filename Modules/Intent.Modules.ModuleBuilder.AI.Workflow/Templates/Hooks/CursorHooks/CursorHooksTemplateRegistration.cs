using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.CursorHooks
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CursorHooksTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => CursorHooksTemplate.TemplateId;

        /// <summary>
        /// There is no per-File-Template "only emit when X" model setting, so suppressing the output
        /// entirely is done by overriding Register and only calling base when the switch is on.
        /// Safe because this class is Mode.Merge and SingleFileTemplateRegistration.Register is a
        /// plain override rather than sealed.
        /// </summary>
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            var settings = Intent.Modules.ModuleBuilder.AI.Workflow.Settings.ModuleSettingsExtensions.GetAIWorkflowSettings(application.Settings);
            if (!settings.InstallAgentGateHooks())
            {
                return;
            }

            base.Register(registry, application);
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new CursorHooksTemplate(outputTarget);
        }
    }
}