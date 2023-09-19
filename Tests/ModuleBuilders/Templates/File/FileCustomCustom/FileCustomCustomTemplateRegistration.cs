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

namespace ModuleBuilders.Templates.File.FileCustomCustom
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class FileCustomCustomTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public FileCustomCustomTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => FileCustomCustomTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            registry.RegisterTemplate(TemplateId, project => new FileCustomCustomTemplate(project, null));
        }
    }
}