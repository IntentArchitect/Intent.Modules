using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiElementModelRegistration : ModelTemplateRegistrationBase<ElementSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiElementModelRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiElementModel.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ElementSettingsModel model)
        {
            var associationSettings = _metadataManager.GetAssociationSettingsModels(project.Application)
                .Where(x => x.TargetEnd.TargetsType(model.Id) || x.SourceEnd.TargetsType(model.Id))
                .ToList();
            return new ApiElementModel(project, model, associationSettings);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ElementSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.GetElementSettingsModels(application)
                .Where(x => !x.Designer.IsReference())
                .ToList();
        }
    }
}