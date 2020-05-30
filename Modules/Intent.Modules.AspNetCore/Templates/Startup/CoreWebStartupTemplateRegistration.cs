using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    [Description(CoreWebStartupTemplate.Identifier)]
    public class CoreWebStartupTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => CoreWebStartupTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new CoreWebStartupTemplate(project, project.Application.EventDispatcher);
        }

        //public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        //{
        //    var targetProjectIds = new List<string>
        //    {
        //        VisualStudioProjectTypeIds.CoreWebApp
        //    };

        //    var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

        //    foreach (var project in projects)
        //    {
        //        registry.Register(TemplateId, project, p => new CoreWebStartupTemplate(project, application.EventDispatcher));
        //    }
        //}
    }
}
