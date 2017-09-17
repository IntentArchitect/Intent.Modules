using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Entities.Templates.DomainPartialEntity
{
    [Description("Intent Entity Partial Template")]
    public class Registration : ModelTemplateRegistrationBase<Class>
    {
        private IMetaDataManager _metaDataManager;

        public Registration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId
        {
            get
            {
                return DomainPartialEntityTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, Class model)
        {
            return new DomainPartialEntityTemplate(model, project);
        }

        public override IEnumerable<Class> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetMetaData<Class>(new MetaDataType("DomainEntity")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
