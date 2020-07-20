using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.Program
{
    [Description(ConsoleAppProgramTemplate.Identifier)]
    public class ConsoleAppProgramTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => ConsoleAppProgramTemplate.Identifier;

        public ConsoleAppProgramTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetConsoleAppNETFrameworkModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new ConsoleAppProgramTemplate(p));
            }
        }
        
    }
}
