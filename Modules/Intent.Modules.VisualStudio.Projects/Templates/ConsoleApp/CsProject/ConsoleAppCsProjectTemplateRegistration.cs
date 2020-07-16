using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.CsProject
{
    [Description(ConsoleAppCsProjectTemplate.Identifier)]
    public class ConsoleAppCsProjectTemplateRegistration : IProjectTemplateRegistration, IProjectRegistration
    {
        public string TemplateId => ConsoleAppCsProjectTemplate.Identifier;
        private readonly IMetadataManager _metadataManager;

        public ConsoleAppCsProjectTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IProjectRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetConsoleAppNETFrameworkModels();
            foreach (var model in models)
            {
                registry.RegisterProject(model.ToProjectConfig());
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetConsoleAppNETFrameworkModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterProjectTemplate(TemplateId, project, p => new ConsoleAppCsProjectTemplate(project, model));
            }
        }
    }
}
