using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    [Description(CoreLibraryCSProjectTemplate.Identifier)]
    public class CoreLibraryCSProjectTemplateRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => CoreLibraryCSProjectTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                ProjectTypeIds.CoreCSharpLibrary
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new CoreLibraryCSProjectTemplate(project));
            }
        }
    }
}
