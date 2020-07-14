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
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiMetadataPackageExtensionsRegistration : ListModelTemplateRegistrationBase<PackageSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataPackageExtensionsRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiMetadataPackageExtensions.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<PackageSettingsModel> model)
        {
            return new ApiMetadataPackageExtensions(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<PackageSettingsModel> GetModels(IApplication application)
        {
            var models = _metadataManager.ModuleBuilder(application).GetPackageSettingsModels().ToList();

            if (!models.Any())
            {
                AbortRegistration();
            }

            return models;

        }
    }
}