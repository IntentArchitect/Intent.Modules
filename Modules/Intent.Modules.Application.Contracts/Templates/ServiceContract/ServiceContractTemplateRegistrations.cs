using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    [Description(ServiceContractTemplate.IDENTIFIER)]
    public class ServiceContractTemplateRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ServiceContractTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => ServiceContractTemplate.IDENTIFIER;

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

