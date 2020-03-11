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
    public class StereotypeDefinitionFileTemplateRegistration : ModelTemplateRegistrationBase<IModuleStereotype>
    {
        private readonly IMetadataManager _metadataManager;

        public StereotypeDefinitionFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => StereotypeDefinitionFileTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IModuleStereotype model)
        {
            return new StereotypeDefinitionFileTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IModuleStereotype> GetModels(IApplication application)
        {
            return _metadataManager.GetModuleStereotypes(application);
        }
    }
}