using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Program;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.Program
{
    [Description(CoreWebProgramTemplate.Identifier)]
    public class ConsoleAppProgramTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => ConsoleAppProgramTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                ProjectTypeIds.ConsoleAppNetFramework
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new ConsoleAppProgramTemplate(project));
            }
        }
    }
}
