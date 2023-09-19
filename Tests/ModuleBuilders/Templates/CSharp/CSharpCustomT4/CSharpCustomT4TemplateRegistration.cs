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
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace ModuleBuilders.Templates.CSharp.CSharpCustomT4
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CSharpCustomT4TemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpCustomT4TemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => CSharpCustomT4Template.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            registry.RegisterTemplate(TemplateId, project => new CSharpCustomT4Template(project, null));
        }
    }
}