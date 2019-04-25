using System.Collections.Generic;
using System.ComponentModel;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    [Description(DomainEntityBehavioursTemplate.Identifier)]
    public class DomainEntityBehavioursTemplateRegistration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public DomainEntityBehavioursTemplateRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DomainEntityBehavioursTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityBehavioursTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }
}
