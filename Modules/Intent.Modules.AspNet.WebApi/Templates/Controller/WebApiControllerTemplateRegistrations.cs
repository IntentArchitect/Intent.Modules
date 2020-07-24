using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.WebApi.Templates.Controller
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

