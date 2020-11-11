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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiPackageModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiPackageModelTemplateRegistration : FilePerModelTemplateRegistration<PackageSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiPackageModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiPackageModelTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, PackageSettingsModel model)
        {
            return new ApiPackageModelTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<PackageSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetPackageSettingsModels();
        }
    }
}