using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;
using System.IO;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    [Description("Visual Studio 2015 Solution- VS Projects")]
    public class Registrations : ApplicationTemplateRegistration
    {

        public override string TemplateId => VisualStudio2015SolutionTemplate.Identifier;

        public override void DoRegistration(IApplicationTemplateInstanceRegistry registry, IApplication application)
        {
            SolutionFile existingSolution = null;
            if (File.Exists(application.GetSolutionPath()))
            {
                existingSolution = SolutionFile.Parse(application.GetSolutionPath());
            }
            registry.RegisterApplicationTemplate(VisualStudio2015SolutionTemplate.Identifier, () => new VisualStudio2015SolutionTemplate(application, existingSolution));
        }

    }
}
