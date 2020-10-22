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

namespace Intent.Modules.Angular.Templates.Model.FormGroupTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class FormGroupTemplateRegistration : FilePerModelTemplateRegistration<FormGroupDefinitionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public FormGroupTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => FormGroupTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, FormGroupDefinitionModel model)
        {
            return new FormGroupTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<FormGroupDefinitionModel> GetModels(IApplication application)
        {
            return _metadataManager.Angular(application).GetFormGroupDefinitionModels();
        }
    }
}