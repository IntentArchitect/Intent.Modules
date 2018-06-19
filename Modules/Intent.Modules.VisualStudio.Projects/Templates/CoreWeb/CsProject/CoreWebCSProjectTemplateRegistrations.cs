using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    [Description(CoreWebCSProjectTemplate.Identifier)]
    public class CoreWebCSProjectTemplateRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => CoreWebCSProjectTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                ProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new CoreWebCSProjectTemplate(project));
            }
        }
    }
}
