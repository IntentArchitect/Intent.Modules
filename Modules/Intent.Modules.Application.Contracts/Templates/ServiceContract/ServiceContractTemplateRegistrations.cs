using Intent.Engine;
using Intent.Templates;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    [Description(ServiceContractTemplate.IDENTIFIER)]
    public class ServiceContractTemplateRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceContractTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceContractTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new ServiceContractTemplate(project, model);
        }

        public override IEnumerable<ServiceModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels();
        }
    }
}

