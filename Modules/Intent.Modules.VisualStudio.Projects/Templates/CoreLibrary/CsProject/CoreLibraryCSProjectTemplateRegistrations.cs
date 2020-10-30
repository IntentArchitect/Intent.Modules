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
    public class CoreLibraryCSProjectTemplateRegistrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => CoreLibraryCSProjectTemplate.Identifier;

        public CoreLibraryCSProjectTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetClassLibraryNETCoreModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new CoreLibraryCSProjectTemplate(p, model));
            }
        }
    }
}
