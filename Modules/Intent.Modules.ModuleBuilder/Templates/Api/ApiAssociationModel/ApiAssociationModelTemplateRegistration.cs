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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiAssociationModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiAssociationModelTemplateRegistration : FilePerModelTemplateRegistration<AssociationSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiAssociationModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiAssociationModelTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, AssociationSettingsModel model)
        {
            return new ApiAssociationModelTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<AssociationSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetAssociationSettingsModels()
                .Where(x => !x.DesignerSettings.IsReference())
                ;
        }
    }
}