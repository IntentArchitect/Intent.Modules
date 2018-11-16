using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Templates.AssemblyInfo
{
    [Description(CoreWebProgramTemplate.Identifier)]
    public class CoreWebProgramTemplateRegistration : IProjectTemplateRegistration
    {
        public string TemplateId => CoreWebProgramTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                VisualStudioProjectTypeIds.CoreWebApp
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new CoreWebProgramTemplate(project));
            }
        }
    }
}
