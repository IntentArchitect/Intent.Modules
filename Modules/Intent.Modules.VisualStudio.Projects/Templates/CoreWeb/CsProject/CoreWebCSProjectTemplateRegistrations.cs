using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    [Description(CoreWebCSProjectTemplate.Identifier)]
    public class CoreWebCSProjectTemplateRegistrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => CoreWebCSProjectTemplate.Identifier;

        public CoreWebCSProjectTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetASPNETCoreWebApplicationModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterTemplate(TemplateId, project, p => new CoreWebCSProjectTemplate(project, model));
            }
            //var targetProjectIds = new List<string>
            //{
            //    VisualStudioProjectTypeIds.CoreWebApp
            //};

            //var projects = application.Projects.Where(p => targetProjectIds.Contains(p.Type));

            //foreach (var project in projects)
            //{
            //    registery.Register(TemplateId, project, p => new CoreWebCSProjectTemplate(project));
            //}
        }
    }
}
