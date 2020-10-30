using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.WebApiServiceCSProjectFile
{
    [Description("Web Api Service CS Project File - VS Projects")] 
    public class Registrations : ITemplateRegistration
    {
        public string TemplateId => WebApiServiceCSProjectFileTemplate.Identifier;
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetASPNETWebApplicationNETFrameworkModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterTemplate(TemplateId, project, p => new WebApiServiceCSProjectFileTemplate(project, model));
            }
        }
    }
}
