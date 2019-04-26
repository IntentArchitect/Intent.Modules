using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.Registrations;
using System.Linq;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.HttpServiceProxy.Legacy.Proxy
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            var clientServiceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataIdentifier("Service-Legacy")).Where(x => x.Clients.Contains(application.ApplicationName)).ToList();

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
