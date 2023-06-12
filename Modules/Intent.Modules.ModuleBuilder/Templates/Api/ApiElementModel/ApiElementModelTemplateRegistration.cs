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

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiElementModelTemplateRegistration : FilePerModelTemplateRegistration<ElementSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiElementModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiElementModelTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ElementSettingsModel model)
        {
            var associationSettings = _metadataManager.ModuleBuilder(outputTarget.Application).GetAssociationSettingsModels()
                .Where(x => x.TargetEnd.TargetsType(model.Id) || x.SourceEnd.TargetsType(model.Id))
                .ToList();
            return new ApiElementModelTemplate(outputTarget, model, associationSettings);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ElementSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetElementSettingsModels()
                .Where(x => !x.DesignerSettings.IsReference())
                .ToList();
        }
    }
}