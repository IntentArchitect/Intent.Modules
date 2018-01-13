using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Other Servers")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public RemoteRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetMetaData<ServiceModel>(new MetaDataIdentifier("Service"));

            serviceModels = serviceModels.Where(x => x.GetStereotypeProperty("Consumers", "CommaSeperatedList", "").Split(',').Any(a => a.Trim() == application.ApplicationName));

            return serviceModels.ToList();
        }
    }
}
