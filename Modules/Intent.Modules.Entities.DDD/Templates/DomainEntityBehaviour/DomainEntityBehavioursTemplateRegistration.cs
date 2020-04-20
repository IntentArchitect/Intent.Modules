using System.Collections.Generic;
using System.ComponentModel;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Templates;


namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    [Description(DomainEntityBehavioursTemplate.Identifier)]
    public class DomainEntityBehavioursTemplateRegistration : ModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEntityBehavioursTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEntityBehavioursTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ClassModel model)
        {
            return new DomainEntityBehavioursTemplate(model, project);
        }

        public override IEnumerable<ClassModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.GetDomainClasses(application.Id);
        }
    }
}
