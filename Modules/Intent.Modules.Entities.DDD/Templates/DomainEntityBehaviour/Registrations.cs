using System.Collections.Generic;
using System.ComponentModel;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    [Description("Intent.Entities.DDD - Behaviour Template")]
    public class Registrations : ModelTemplateRegistrationBase<IClass>
    {
        private IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
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
