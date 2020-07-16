﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    [Description(AppSettingsTemplate.Identifier)]
    public class AppSettingsTemplateRegistration : IProjectTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => AppSettingsTemplate.Identifier;


        public AppSettingsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetASPNETCoreWebApplicationModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterProjectTemplate(TemplateId, project, p => new AppSettingsTemplate(project, project.Application.EventDispatcher));
            }
        }
    }
}
