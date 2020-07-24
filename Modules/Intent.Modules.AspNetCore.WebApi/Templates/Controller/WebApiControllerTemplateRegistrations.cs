using System.Collections.Generic;
using System.ComponentModel;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Templates;


namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
{
    [Description(WebApiControllerTemplate.Identifier)]
    public class WebApiControllerTemplateRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        public WebApiControllerTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => WebApiControllerTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new WebApiControllerTemplate(project, model);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels();
        }
    }
}

