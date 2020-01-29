using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;


namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.LaunchSettings
{
    [Description(LaunchSettingsJsonTemplate.Identifier)]
    public class LaunchSettingsJsonTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => LaunchSettingsJsonTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                VisualStudioProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.Type));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new LaunchSettingsJsonTemplate(project, application.EventDispatcher));
            }
        }
    }
}
