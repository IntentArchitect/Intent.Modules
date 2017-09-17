using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Microsoft.Build.Construction;
using System;
using System.IO;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
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
