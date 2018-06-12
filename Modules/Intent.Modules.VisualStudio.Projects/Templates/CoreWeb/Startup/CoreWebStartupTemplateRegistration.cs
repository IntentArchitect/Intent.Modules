using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup
{
    [Description(CoreWebStartupTemplate.Identifier)]
    public class CoreWebStartupTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => CoreWebStartupTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                ProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new CoreWebStartupTemplate(project, application.EventDispatcher));
            }
        }
    }
}
