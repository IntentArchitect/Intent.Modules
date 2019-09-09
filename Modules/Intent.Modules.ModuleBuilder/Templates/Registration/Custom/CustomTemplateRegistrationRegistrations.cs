using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Helpers;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.Custom
{
    public class CustomTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IModuleBuilderElement>
    {
        private readonly IMetadataManager _metadataManager;

        public CustomTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CustomTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IModuleBuilderElement model)
        {
            return new CustomTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IModuleBuilderElement> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetAllElements(applicationManager)
                .Where(x => (x.IsCSharpTemplate() || x.IsFileTemplate()) && x.GetCreationMode() == CreationMode.Custom)
                .ToList();
        }
    }
}
