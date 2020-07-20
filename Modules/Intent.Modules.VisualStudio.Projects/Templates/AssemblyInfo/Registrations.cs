
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Templates;
using Intent.Registrations;
using System.ComponentModel;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Templates.AssemblyInfo
{
    [Description("Assembly Info Template - VS Projects")]
    public class Registrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => AssemblyInfoTemplate.Identifier;


        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var targetProjectIds = new List<string>() {
                VisualStudioProjectTypeIds.ConsoleAppNetFramework,
                VisualStudioProjectTypeIds.CSharpLibrary,
                VisualStudioProjectTypeIds.WcfApplication,
                VisualStudioProjectTypeIds.WebApiApplication };
            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.Type));

            foreach (var project in projects)
            {
                registry.RegisterTemplate(TemplateId, project, (p) => new AssemblyInfoTemplate((IProject) project));
            }
        }
    }
}
