using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Local Server")]
    public class LocalRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public LocalRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.LocalIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.LocalIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetMetaData<ServiceModel>(new MetaDataIdentifier("Service")).Where(x => x.Application.Name == application.ApplicationName);

            // TODO JL: Temp, filter out ones for server only, will ultimately get replaced with concept of client applications in the future
            serviceModels = serviceModels.Where(x => x.Stereotypes.All(s => s.Name != "ServerOnly"));

            return serviceModels.ToList();
        }
    }
}