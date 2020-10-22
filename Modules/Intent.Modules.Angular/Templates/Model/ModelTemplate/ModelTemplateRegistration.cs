using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Model.ModelTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ModelTemplateRegistration : FilePerModelTemplateRegistration<ModelDefinitionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ModelTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, ModelDefinitionModel model)
        {
            return new ModelTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ModelDefinitionModel> GetModels(IApplication application)
        {
            return _metadataManager.Angular(application).GetModelDefinitionModels();
        }
    }
}