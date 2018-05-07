using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    [Description("Intent.Entities - Entity State Template")]
    public class Registrations : ModelTemplateRegistrationBase<IClass>
    {
        private IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId
        {
            get
            {
                return DomainEntityStateTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityStateTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }
}
