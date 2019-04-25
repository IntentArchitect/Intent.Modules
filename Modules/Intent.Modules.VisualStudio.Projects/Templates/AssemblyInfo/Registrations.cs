
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Registrations;
using System.ComponentModel;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Templates.AssemblyInfo
{
    [Description("Assembly Info Template - VS Projects")]
    public class Registrations : IProjectTemplateRegistration
    {
        private readonly IMetadataManager _metaDataManager;

        public Registrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => AssemblyInfoTemplate.Identifier;


        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var targetProjectIds = new List<string>() {
                VisualStudioProjectTypeIds.ConsoleAppNetFramework,
                VisualStudioProjectTypeIds.CSharpLibrary,
                VisualStudioProjectTypeIds.WcfApplication,
                VisualStudioProjectTypeIds.WebApiApplication };
            var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

            foreach (var project in projects)
            {
                registery.Register(TemplateId, project, (p) => new AssemblyInfoTemplate(project));
            }
        }
    }
}
