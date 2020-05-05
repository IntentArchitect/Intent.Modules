using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    [Description(CoreLibraryCSProjectTemplate.Identifier)]
    public class CoreLibraryCSProjectTemplateRegistrations : IProjectTemplateRegistration, IProjectRegistration
    {
        private IMetadataManager _metadataManager;
        public string TemplateId => CoreLibraryCSProjectTemplate.Identifier;

        public CoreLibraryCSProjectTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IProjectRegistry registry, IApplication application)
        {
            var models = _metadataManager.GetClassLibraryDotNetCoreProjects(application.Id);
            foreach (var model in models)
            {
                registry.RegisterProject(model.ToProjectConfig());
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            var models = _metadataManager.GetClassLibraryDotNetCoreProjects(application.Id);

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registery.Register(TemplateId, project, p => new CoreLibraryCSProjectTemplate(project, model));
            }
        }
    }
}
