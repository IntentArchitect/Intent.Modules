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
    public class ApiPackageModelRegistration : FilePerModelTemplateRegistration<PackageSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiPackageModelRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiPackageModel.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, PackageSettingsModel model)
        {
            return new ApiPackageModel(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<PackageSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetPackageSettingsModels();
        }
    }
}