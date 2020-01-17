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
    public class CustomTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<ITemplateDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public CustomTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CustomTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ITemplateDefinition model)
        {
            return new CustomTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<ITemplateDefinition> GetModels(IApplication application)
        {
            return _metadataManager.GetTemplateDefinitions(application)
                .Where(x => x.GetCreationMode() == CreationMode.Custom)
                .ToList();
        }
    }
}
