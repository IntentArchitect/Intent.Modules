using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.CsProject;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleAppCsProjectFile
{
    [Description(ConsoleAppCsProjectTemplate.Identifier)]
    public class ConsoleAppCsProjectTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => ConsoleAppCsProjectTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                VisualStudioProjectTypeIds.ConsoleAppNetFramework
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new ConsoleAppCsProjectTemplate(project));
            }
        }
    }
}
