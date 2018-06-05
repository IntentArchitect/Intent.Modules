using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    [Description(ServiceContractTemplate.Identifier)]
    public class ServiceContractTemplateRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ServiceContractTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => ServiceContractTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new ServiceContractTemplate(project, model);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetServiceModels(application);
        }
    }
}

