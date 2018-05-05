using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.Registrations.Templates.OneTemplateToManyModels
{
    [Description("Templates.Registrations.OneTemplateToManyModels Template")]
    public class Registration : ListModelTemplateRegistrationBase<IClass>
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
                return Template.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, IList<IClass>  models)
        {
            return new Template(project, models);
        }

        public override IList<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application).ToList();
        }
    }
}
