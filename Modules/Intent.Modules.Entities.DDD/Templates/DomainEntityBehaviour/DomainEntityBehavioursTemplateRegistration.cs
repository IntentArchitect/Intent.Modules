using System.Collections.Generic;
using System.ComponentModel;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates


namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    [Description(DomainEntityBehavioursTemplate.Identifier)]
    public class DomainEntityBehavioursTemplateRegistration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metaDataManager;

        public DomainEntityBehavioursTemplateRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DomainEntityBehavioursTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityBehavioursTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }
}
