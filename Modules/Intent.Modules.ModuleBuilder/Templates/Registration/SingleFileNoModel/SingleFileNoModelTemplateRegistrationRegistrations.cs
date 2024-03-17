using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileNoModel
{
    public class SingleFileNoModelTemplateRegistrationRegistrations : FilePerModelTemplateRegistration<TemplateRegistrationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SingleFileNoModelTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SingleFileNoModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, TemplateRegistrationModel model)
        {
            return new SingleFileNoModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<TemplateRegistrationModel> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.ReferencesSingleFile())
                .Select(x => new TemplateRegistrationModel(x))
                .Where(x => x.GetTemplateSettings().ModelType() == null && x.GetTemplateSettings().ModelName() == null)
                .ToList();
        }
    }
}