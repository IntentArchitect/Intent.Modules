using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Registrations;


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
                VisualStudioProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.Type.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new AppSettingsTemplate(project, application.EventDispatcher));
            }
        }
    }
}
