using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject;
using Intent.Registrations;
using Intent.Templates;


namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.LaunchSettings
{
    [Description(LaunchSettingsJsonTemplate.Identifier)]
    public class LaunchSettingsJsonTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => LaunchSettingsJsonTemplate.Identifier;

        public LaunchSettingsJsonTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetASPNETCoreWebApplicationModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new LaunchSettingsJsonTemplate(p, project.Application.EventDispatcher));
            }
        }
    }
}
