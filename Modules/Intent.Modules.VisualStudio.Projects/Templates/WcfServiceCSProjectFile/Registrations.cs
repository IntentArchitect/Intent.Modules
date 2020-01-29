using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Configuration;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.WcfServiceCSProjectFile
{
    [Description("Wcf Service CS Project File - VS Projects")]
    public class Registrations : IProjectTemplateRegistration, IProjectRegistration
    {
        public string TemplateId => WcfServiceCSProjectFileTemplate.IDENTIFIER;
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IProjectRegistry registry, IApplication application)
        {
            var models = _metadataManager.GetWcfServiceApplicationDotNetFrameworkProjects(application.Id);
            foreach (var model in models)
            {
                registry.RegisterProject(model.ToProjectConfig());
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.GetWcfServiceApplicationDotNetFrameworkProjects(application.Id);

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterProjectTemplate(TemplateId, project, p => new WcfServiceCSProjectFileTemplate(project, model));
            }
        }
    }
}
