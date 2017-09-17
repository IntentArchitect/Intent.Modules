using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.HttpServiceProxy.Legacy.Proxy
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var clientServiceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy")).Where(x => x.Clients.Contains(application.ApplicationName)).ToList();

            foreach (var serviceModel in clientServiceModels)
            {
                if (serviceModel.DistributionMode != ServiceDistributionMode.WebApi)
                {
                    return;
                }

                if (serviceModel.ApplicationName != application.ApplicationName)
                {
                    RegisterTemplate(WebApiClientServiceProxyTemplate.Identifier, project => new WebApiClientServiceProxyTemplate(serviceModel, project));
                }
            }
        }
    }
}
