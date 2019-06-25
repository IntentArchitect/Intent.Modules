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
    public class ServiceContractTemplateRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly ServicesMetadataProvider _metadataManager;

        public ServiceContractTemplateRegistrations(ServicesMetadataProvider metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceContractTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new ServiceContractTemplate(project, model);
        }

        public override IEnumerable<IServiceModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.GetServices(application);
        }
    }
}

