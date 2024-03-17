using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    public class SingleFileListModelTemplateRegistrationRegistrations : FilePerModelTemplateRegistration<TemplateRegistrationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SingleFileListModelTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SingleFileListModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, TemplateRegistrationModel model)
        {
            return new SingleFileListModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<TemplateRegistrationModel> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.ReferencesSingleFile())
                .Select(x => new TemplateRegistrationModel(x))
                .Where(x => x.GetTemplateSettings().ModelType() != null || x.GetTemplateSettings().ModelName() != null)
                .ToList();
        }
    }
}