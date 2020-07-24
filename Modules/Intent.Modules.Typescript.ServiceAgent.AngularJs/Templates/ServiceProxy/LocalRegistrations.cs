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

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Local Server")]
    public class LocalRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        public LocalRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.LocalIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.LocalIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metadataManager.Services(application).GetServiceModels();

            // TODO JL: Temp, filter out ones for server only, will ultimately get replaced with concept of client applications in the future
            serviceModels = serviceModels.Where(x => x.Stereotypes.All(s => s.Name != "ServerOnly")).ToList();

            return serviceModels;
        }
    }
}