using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.LibraryCSProjectFile
{
    [Description("Library CS Project - VS Projects")]
    public class Registrations : IProjectTemplateRegistration, IProjectRegistration
    {
        public string TemplateId => LibraryCSProjectFileTemplate.IDENTIFIER;
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IProjectRegistry registry, IApplication application)
        {
            var models = _metadataManager.GetClassLibraryDotNetFrameworkProjects(application.Id);
            foreach (var model in models)
            {
                registry.RegisterProject(model.ToProjectConfig());
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var models = _metadataManager.GetClassLibraryDotNetFrameworkProjects(application.Id);

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registery.Register(TemplateId, project, p => new LibraryCSProjectFileTemplate(project, model));
            }
        }
    }
}
