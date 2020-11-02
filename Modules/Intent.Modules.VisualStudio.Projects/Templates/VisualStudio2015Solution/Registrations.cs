using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using System.IO;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    [Description("Visual Studio 2015 Solution- VS Projects")]
    public class Registrations : IApplicationTemplateRegistration
    {

        public string TemplateId => VisualStudio2015SolutionTemplate.Identifier;
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(IApplicationTemplateInstanceRegistry registry, IApplication application)
        {
            var vsSolutions = _metadataManager.VisualStudio(application).GetVisualStudioSolutionModels();
            foreach (var vsSolution in vsSolutions)
            {
                var projects = _metadataManager.GetAllProjectModels(application).Where(x => x.Solution.Id == vsSolution.Id).ToList();
                registry.RegisterApplicationTemplate(VisualStudio2015SolutionTemplate.Identifier, () => new VisualStudio2015SolutionTemplate(application, vsSolution, projects));
            }
        }
    }
}
