using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebApiServiceCSProjectFile
{
    [Description("Web Api Service CS Project File - VS Projects")] 
    public class Registrations : IProjectTemplateRegistration
    {
        public string TemplateId => WebApiServiceCSProjectFileTemplate.IDENTIFIER;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                ProjectTypeIds.WebApiApplication
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new WebApiServiceCSProjectFileTemplate(project));
            }
        }
    }
}
