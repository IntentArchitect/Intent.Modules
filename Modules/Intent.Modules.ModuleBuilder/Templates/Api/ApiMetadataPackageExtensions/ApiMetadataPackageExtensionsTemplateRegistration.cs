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
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiMetadataPackageExtensionsTemplateRegistration : SingleFileListModelTemplateRegistration<PackageSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataPackageExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiMetadataPackageExtensionsTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<PackageSettingsModel> model)
        {
            return new ApiMetadataPackageExtensionsTemplate(outputTarget, model);
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