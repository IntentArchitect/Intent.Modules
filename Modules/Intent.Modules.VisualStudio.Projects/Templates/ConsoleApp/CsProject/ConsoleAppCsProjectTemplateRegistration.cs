﻿using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.CsProject
{
    [Description(ConsoleAppCsProjectTemplate.Identifier)]
    public class ConsoleAppCsProjectTemplateRegistration : ITemplateRegistration, IOutputTargetRegistration
    {
        public string TemplateId => ConsoleAppCsProjectTemplate.Identifier;
        private readonly IMetadataManager _metadataManager;

        public ConsoleAppCsProjectTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IOutputTargetRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetConsoleAppNETFrameworkModels();
            foreach (var model in models)
            {
                registry.RegisterOutputTarget(model.ToProjectConfig());
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetConsoleAppNETFrameworkModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new ConsoleAppCsProjectTemplate(p, model));
            }
        }
    }
}
