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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiMetadataProviderExtensionsTemplateRegistration : SingleFileListModelTemplateRegistration<ElementSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProviderExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiMetadataProviderExtensionsTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<ElementSettingsModel> model)
        {
            return new ApiMetadataProviderExtensionsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ElementSettingsModel> GetModels(IApplication application)
        {
            var models = _metadataManager.ModuleBuilder(application).GetElementSettingsModels()
                .Where(x => x.MustSaveInOwnFile() && !x.DesignerSettings.IsReference())
                .ToList();

            if (!models.Any())
            {
                AbortRegistration();
            }

            return models;
        }
    }
}