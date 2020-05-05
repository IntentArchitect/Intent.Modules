using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Constants;
using Intent.Registrations;

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    [Description(CoreWebStartupTemplate.Identifier)]
    public class CoreWebStartupTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => CoreWebStartupTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                VisualStudioProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registry.Register(TemplateId, project, p => new CoreWebStartupTemplate(project, application.EventDispatcher));
            }
        }
    }
}
