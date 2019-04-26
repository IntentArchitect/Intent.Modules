using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject;
using Intent.Engine;
using Intent.Registrations;


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
                VisualStudioProjectTypeIds.CoreCSharpLibrary
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new CoreLibraryCSProjectTemplate(project));
            }
        }
    }
}
