using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    [Description("Intent Applications Service Contracts (Clients)")]
    public class ServiceContractRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CSharp";

        public ServiceContractRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateIds.ClientServiceContract;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new Contracts.Templates.ServiceContract.ServiceContractTemplate(project, model, TemplateId);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            var serviceModels = new ServicesMetadataProvider(_metadataManager).GetAllServices().ToArray();

            return serviceModels
                .Where(x => x.GetConsumers(_stereotypeName, _stereotypePropertyName).Any(y => y.Equals(application.Name, StringComparison.OrdinalIgnoreCase)))
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