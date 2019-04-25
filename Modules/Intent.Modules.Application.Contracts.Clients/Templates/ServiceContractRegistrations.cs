using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    [Description("Intent Applications Service Contracts (Clients)")]
    public class ServiceContractRegistrations : ModelTemplateRegistrationBase<MetaModel.Service.IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CSharp";

        public ServiceContractRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TemplateIds.ClientServiceContract;

        public override ITemplate CreateTemplateInstance(IProject project, MetaModel.Service.IServiceModel model)
        {
            return new Contracts.Templates.ServiceContract.ServiceContractTemplate(project, model, TemplateId);
        }

        public override IEnumerable<MetaModel.Service.IServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetMetaData<MetaModel.Service.IServiceModel>("Services").ToArray();
            if (!serviceModels.Any())
            {
                serviceModels = _metaDataManager.GetMetaData<MetaModel.Service.IServiceModel>("Service").ToArray(); // backward compatibility
            }

            return serviceModels
                .Where(x => x.GetConsumers(_stereotypeName, _stereotypePropertyName).Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            settings.SetIfSupplied("Consumer Stereotype Name", (s) => _stereotypeName = s);
            settings.SetIfSupplied("Consumer Stereotype Property Name", (s) => _stereotypePropertyName = s);
        }
    }
}