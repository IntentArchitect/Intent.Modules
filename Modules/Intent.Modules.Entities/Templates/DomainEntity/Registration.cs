using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Entities.Templates.DomainEntity;

namespace Intent.Modules.Entities.Templates.DomainPartialEntity
{
    [Description("Intent.Entities - Entity Template")]
    public class Registration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DomainEntityTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }
}
