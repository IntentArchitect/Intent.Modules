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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiAssociationModelExtensionsTemplateRegistration : FilePerModelTemplateRegistration<AssociationSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiAssociationModelExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiAssociationModelExtensionsTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, AssociationSettingsModel model)
        {
            return new ApiAssociationModelExtensionsTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<AssociationSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetAssociationSettingsModels();
        }
    }
}