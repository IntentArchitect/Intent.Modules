using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Engine;
using Microsoft.Build.Construction;
using System.IO;

namespace Intent.SoftwareFactory.VSProjects.Templates.VisualStudio2015Solution
{
    public class VisualStudio2015SolutionRegistration : ApplicationTemplateRegistration
    { 
        public override void DoRegistrations(Action<ITemplate> register, IApplication application)
        {
            SolutionFile existingSolution = null;
            if (File.Exists(application.GetSolutionPath()))
            {
                existingSolution = SolutionFile.Parse(application.GetSolutionPath());
            }
            register(new VisualStudio2015SolutionTemplate(application, existingSolution));
        }
    }
}
