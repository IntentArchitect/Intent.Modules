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
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace ModuleBuilders.Templates.TypeScript.TypeScriptCustomCustom
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TypeScriptCustomCustomTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public TypeScriptCustomCustomTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => TypeScriptCustomCustomTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            registry.RegisterTemplate(TemplateId, project => new TypeScriptCustomCustomTemplate(project, null));
        }
    }
}