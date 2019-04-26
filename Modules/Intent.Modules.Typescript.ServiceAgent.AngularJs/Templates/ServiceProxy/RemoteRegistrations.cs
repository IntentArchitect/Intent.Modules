using System;
using Intent.Modelers.Services.Api;
using Intent.Engine;
using Intent.Templates
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Other Servers")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metaDataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CommaSeperatedList";

        public RemoteRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetMetaData<IServiceModel>("Services");

            serviceModels = serviceModels.Where(x => GetConsumers(x).Any(y => application.ApplicationName.Equals(y, StringComparison.OrdinalIgnoreCase)));

            return serviceModels.ToList();
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            settings.SetIfSupplied("Consumer Stereotype Name", (s) => _stereotypeName = s);
            settings.SetIfSupplied("Consumer Stereotype Property Name", (s) => _stereotypePropertyName = s);
        }

        private IEnumerable<string> GetConsumers(IServiceModel dto)
        {
            return dto.HasStereotype(_stereotypeName)
                ? dto.GetStereotypeProperty(_stereotypeName, _stereotypePropertyName, "").Split(',').Select(x => x.Trim()).ToArray()
                : dto.GetStereotypeInFolders(_stereotypeName).GetProperty(_stereotypePropertyName, "").Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
