﻿using System.Collections.Generic;
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
    public class Registrations : ITemplateRegistration, IOutputTargetRegistration
    {
        public string TemplateId => WcfServiceCSProjectFileTemplate.IDENTIFIER;
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IOutputTargetRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetWCFServiceApplicationModels();
            foreach (var model in models)
            {
                registry.RegisterOutputTarget(model.ToProjectConfig());
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetWCFServiceApplicationModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterTemplate(TemplateId, project, p => new WcfServiceCSProjectFileTemplate(project, model));
            }
        }
    }
}
