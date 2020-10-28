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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementExtensionModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiElementExtensionModelRegistration : FilePerModelTemplateRegistration<ElementExtensionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiElementExtensionModelRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiElementExtensionModel.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ElementExtensionModel model)
        {
            return new ApiElementExtensionModel(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ElementExtensionModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetElementExtensionModels();
        }
    }
}