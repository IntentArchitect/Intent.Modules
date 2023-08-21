using System;
using System.Collections.Generic;
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

namespace Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorRegistration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TemplateDecoratorRegistrationTemplateRegistration : FilePerModelTemplateRegistration<TemplateDecoratorModel>
    {
        private readonly IMetadataManager _metadataManager;

        public TemplateDecoratorRegistrationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateDecoratorRegistrationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, TemplateDecoratorModel model)
        {
            return new TemplateDecoratorRegistrationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<TemplateDecoratorModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetTemplateDecoratorModels();
        }
    }
}