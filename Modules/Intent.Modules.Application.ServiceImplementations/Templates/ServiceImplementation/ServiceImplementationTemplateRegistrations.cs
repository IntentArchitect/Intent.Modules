using System.Collections.Generic;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Services;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    [Description(ServiceImplementationTemplate.Identifier)]
    public class ServiceImplementationTemplateRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly ApiMetadataProvider _metadataManager;

        public ServiceImplementationTemplateRegistrations(ApiMetadataProvider metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceImplementationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new ServiceImplementationTemplate(project, model);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            return _metadataManager.GetServiceModels(application);
        }
    }
}

