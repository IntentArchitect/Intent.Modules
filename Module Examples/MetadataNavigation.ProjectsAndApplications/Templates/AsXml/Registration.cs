using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataNavigation.ProjectsAndApplications.Templates.AsXml
{
    [Description("MetadataNavigation.ProjectsAndApplications.AsXml Template")]
    public class Registration : ModelTemplateRegistrationBase<IApplication>
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

        public override ITemplate CreateTemplateInstance(IProject project, Intent.SoftwareFactory.Engine.IApplication model)
        {
            return new Template(project, model);
        }

        public override IEnumerable<IApplication> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return new[] { application };
        }
    }

}

