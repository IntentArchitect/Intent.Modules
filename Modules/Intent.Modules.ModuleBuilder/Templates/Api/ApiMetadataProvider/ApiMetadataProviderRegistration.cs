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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProvider
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiMetadataProviderRegistration : ListModelTemplateRegistrationBase<IElementSettings>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProviderRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiMetadataProvider.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IElementSettings> model)
        {
            return new ApiMetadataProvider(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<IElementSettings> GetModels(IApplication application)
        {
            return _metadataManager.GetElementSettings(application).Where(x => !x.IsChild).ToList();
        }
    }
}