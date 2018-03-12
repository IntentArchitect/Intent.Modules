using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory;

namespace Templates.Registrations.Templates.CustomBinding
{
    public class Registration : IProjectTemplateRegistration
    {
        private IMetaDataManager _metaDataManager;

        public Registration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }


        public string TemplateId
        {
            get
            {
                return Template.Identifier;
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            //Output 1 template instance per project
            foreach (var project in application.Projects)
            {
                registery.Register(TemplateId, project, (p) => new Template(p));
            }

            /*
            
            here is an example with data binding

            var umlModels = _metaDataManager.GetDomainModels(application);

            foreach (var model in umlModels)
            {
                registery.Register(TemplateId, (p) => new Template(p, model));
            }
            */
        }
    }
}
