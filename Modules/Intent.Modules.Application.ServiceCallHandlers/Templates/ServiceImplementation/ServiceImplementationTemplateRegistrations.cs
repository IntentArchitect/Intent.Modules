using Intent.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Services;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceImplementation
{
    [Description(ServiceImplementationTemplate.Identifier)]
    public class ServiceImplementationTemplateRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly ApiMetadataProvider _metadataProvider;

        public ServiceImplementationTemplateRegistrations(ApiMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public override string TemplateId => ServiceImplementationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new ServiceImplementationTemplate(project, model);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            return _metadataProvider.GetServiceModels(application);
        }
    }
}

