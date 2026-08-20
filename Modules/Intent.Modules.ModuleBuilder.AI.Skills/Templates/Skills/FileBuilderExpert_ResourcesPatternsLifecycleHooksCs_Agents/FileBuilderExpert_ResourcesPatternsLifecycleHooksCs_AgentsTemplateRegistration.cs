using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.FileBuilderExpert_ResourcesPatternsLifecycleHooksCs_Agents
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class FileBuilderExpert_ResourcesPatternsLifecycleHooksCs_AgentsTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => FileBuilderExpert_ResourcesPatternsLifecycleHooksCs_AgentsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new FileBuilderExpert_ResourcesPatternsLifecycleHooksCs_AgentsTemplate(outputTarget);
        }
    }
}