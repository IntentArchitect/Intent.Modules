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
    public class CustomTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<ITemplateRegistration>
    {
        private readonly IMetadataManager _metadataManager;

        public CustomTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CustomTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ITemplateRegistration model)
        {
            return new CustomTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<ITemplateRegistration> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IElement>("Module Builder")
                .Where(x => x.ReferencesCustom())
                .Select(x => new TemplateRegistration(x))
                .ToList();
        }
    }
}
