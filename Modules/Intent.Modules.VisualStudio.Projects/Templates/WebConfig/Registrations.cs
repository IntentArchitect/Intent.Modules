using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebConfig
{
    [Description("Web Config - VS Projects")]
    public class Registrations : IProjectTemplateRegistration
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => WebApiWebConfigFileTemplate.IDENTIFIER;


        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>() {
                ProjectTypeIds.WebApiApplication,
                ProjectTypeIds.WcfApplication
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, (p) => new WebApiWebConfigFileTemplate(project, application.EventDispatcher));
            }
        }
    }
}
