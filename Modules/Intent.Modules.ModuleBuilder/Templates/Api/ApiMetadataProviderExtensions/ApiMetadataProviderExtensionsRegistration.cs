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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiMetadataProviderExtensionsRegistration : ListModelTemplateRegistrationBase<ElementSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProviderExtensionsRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiMetadataProviderExtensions.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<ElementSettingsModel> model)
        {
            return new ApiMetadataProviderExtensions(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ElementSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.GetElementSettingsModels(application)
                .Where(x => x.GetSettings().SaveMode().IsOwnFile() && !x.Designer.GetModelerSettings().ModelerType().IsReference())
                .ToList();
        }
    }
}