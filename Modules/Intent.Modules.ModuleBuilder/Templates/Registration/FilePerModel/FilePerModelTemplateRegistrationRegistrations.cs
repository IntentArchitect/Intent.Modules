using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.FilePerModel
{
    public class FilePerModelTemplateRegistrationRegistrations : FilePerModelTemplateRegistration<TemplateRegistrationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public FilePerModelTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => FilePerModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, TemplateRegistrationModel model)
        {
            return new FilePerModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<TemplateRegistrationModel> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IElement>("Module Builder", application.Id)
                .Where(x => x.ReferencesFilePerModel())
                .Select(x => new TemplateRegistrationModel(x))
                .ToList();
        }
    }
}