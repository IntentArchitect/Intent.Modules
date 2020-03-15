using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.metadata.StereotypeDefinitionFileTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class StereotypeDefinitionFileTemplateRegistration : ModelTemplateRegistrationBase<IStereotypeDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public StereotypeDefinitionFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => StereotypeDefinitionFileTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IStereotypeDefinition model)
        {
            return new StereotypeDefinitionFileTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IStereotypeDefinition> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IStereotypeDefinition>("Module Builder", application.Id)
                .Where(x => x.GetParentPath().Any(p => p.SpecializationType == ModulePackage.SpecializationType))
                .ToList();
        }
    }
}