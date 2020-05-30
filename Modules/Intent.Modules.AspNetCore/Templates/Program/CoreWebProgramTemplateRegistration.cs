
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Templates;
using Intent.Registrations;
using System.ComponentModel;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Templates.AssemblyInfo
{
    [Description(CoreWebProgramTemplate.Identifier)]
    public class CoreWebProgramTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => CoreWebProgramTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new CoreWebProgramTemplate(project);
        }

        //public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        //{
        //    var targetProjectIds = new List<string>
        //    {
        //        VisualStudioProjectTypeIds.CoreWebApp
        //    };

        //    var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

        //    foreach (var project in projects)
        //    {
        //        registery.Register(TemplateId, project, p => new CoreWebProgramTemplate(project));
        //    }
        //}
    }
}
