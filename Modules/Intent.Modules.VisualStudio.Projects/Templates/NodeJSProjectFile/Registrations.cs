using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.NodeJSProjectFile
{
    [Description("Node JS Project File - VS Projects")]
    public class Registrations : IProjectTemplateRegistration
    {
        public string TemplateId => NodeJSProjectFileTemplate.IDENTIFIER;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>
            {
                VisualStudioProjectTypeIds.NodeJsConsoleApplication,
            };

            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, p => new NodeJSProjectFileTemplate(project));
            }
        }
    }
}
