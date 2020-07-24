using System.Collections.Generic;
using System.ComponentModel;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Templates;


namespace Intent.Modules.Entities.Templates.DomainEntity
{
    [Description(DomainEntityTemplate.Identifier)]
    public class DomainEntityTemplateRegistration : ModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEntityTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEntityTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ClassModel model)
        {
            return new DomainEntityTemplate(model, project);
        }

        public override IEnumerable<ClassModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels();
        }
    }
}
