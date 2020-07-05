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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiMetadataDesignerExtensionsRegistration : ListModelTemplateRegistrationBase<DesignerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataDesignerExtensionsRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiMetadataDesignerExtensions.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<DesignerModel> model)
        {
            return new ApiMetadataDesignerExtensions(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<DesignerModel> GetModels(IApplication application)
        {
            var designers = _metadataManager.ModuleBuilder(application).GetDesignerModels();
            if (!designers.Any())
            {
                AbortRegistration();
            }

            return designers;
        }
    }
}