using Intent.Engine;
using Intent.Registrations;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebConfig
{
    [Description("Web Config - VS Projects")]
    public class Registrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => WebApiWebConfigFileTemplate.IDENTIFIER;


        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>() {
                VisualStudioProjectTypeIds.WebApiApplication,
                VisualStudioProjectTypeIds.WcfApplication
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.Type));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, (p) => new WebApiWebConfigFileTemplate(p, application.EventDispatcher));
            }
        }
    }
}
