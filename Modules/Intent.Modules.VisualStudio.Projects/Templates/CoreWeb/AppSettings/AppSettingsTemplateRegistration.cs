using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    [Description(AppSettingsTemplate.Identifier)]
    public class AppSettingsTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => AppSettingsTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                ProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new AppSettingsTemplate(project, application.EventDispatcher));
            }
        }
    }
}
