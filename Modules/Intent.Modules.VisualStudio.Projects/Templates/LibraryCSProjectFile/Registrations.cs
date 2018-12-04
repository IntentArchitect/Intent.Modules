using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.LibraryCSProjectFile
{
    [Description("Library CS Project - VS Projects")]
    public class Registrations : IProjectTemplateRegistration
    {
        public string TemplateId => LibraryCSProjectFileTemplate.IDENTIFIER;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                VisualStudioProjectTypeIds.CSharpLibrary
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new LibraryCSProjectFileTemplate(project));
            }
        }
    }
}
