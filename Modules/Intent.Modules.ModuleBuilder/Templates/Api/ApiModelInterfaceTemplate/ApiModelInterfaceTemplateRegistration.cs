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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiModelInterfaceTemplateRegistration : ModelTemplateRegistrationBase<ElementSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiModelInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiModelInterfaceTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ElementSettingsModel model)
        {
            return new ApiModelInterfaceTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ElementSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.GetElementSettings(application);
        }
    }
}