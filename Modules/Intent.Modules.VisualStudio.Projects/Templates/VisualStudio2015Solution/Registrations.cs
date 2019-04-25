using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using System.IO;
using Intent.Engine;
using Intent.Registrations;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    [Description("Visual Studio 2015 Solution- VS Projects")]
    public class Registrations : IApplicationTemplateRegistration
    {

        public string TemplateId => VisualStudio2015SolutionTemplate.Identifier;

        public void DoRegistration(IApplicationTemplateInstanceRegistry registry, IApplication application)
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
