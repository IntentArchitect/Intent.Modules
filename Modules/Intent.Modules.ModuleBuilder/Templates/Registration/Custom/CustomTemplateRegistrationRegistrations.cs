using Intent.Metadata.Models;
using Intent.Modules.Common.Registrations;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.Custom
{
    public class CustomTemplateRegistrationRegistrations : FilePerModelTemplateRegistration<TemplateRegistrationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CustomTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CustomTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, TemplateRegistrationModel model)
        {
            return new CustomTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<TemplateRegistrationModel> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.ReferencesCustom())
                .Select(x => new TemplateRegistrationModel(x))
                .ToList();
        }
    }
}
