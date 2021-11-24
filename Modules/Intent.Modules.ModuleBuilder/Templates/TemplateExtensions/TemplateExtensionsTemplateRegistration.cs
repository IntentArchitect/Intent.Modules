using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Registration.FilePerModel;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Templates.TemplateExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TemplateExtensionsTemplateRegistration : SingleFileTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public TemplateExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateExtensionsTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new TemplateExtensionsTemplate(project);
        }
    }
}