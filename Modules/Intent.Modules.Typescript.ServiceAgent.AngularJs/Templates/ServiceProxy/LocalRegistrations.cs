using Intent.Modelers.Services.Api;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Local Server")]
    public class LocalRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public LocalRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.LocalIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.LocalIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetServiceModels(application);

            // TODO JL: Temp, filter out ones for server only, will ultimately get replaced with concept of client applications in the future
            serviceModels = serviceModels.Where(x => x.Stereotypes.All(s => s.Name != "ServerOnly"));

            return serviceModels.ToList();
        }
    }
}