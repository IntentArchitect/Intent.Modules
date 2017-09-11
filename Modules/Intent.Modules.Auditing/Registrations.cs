using System.Linq;
using Intent.Packages.Auditing.Decorators;
using Intent.Packages.Auditing.Templates.HttpRequestMessageExtensions;
using Intent.Packages.Auditing.Templates.ServiceBoundaryAudtingStrategy;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.Modules.WebApi.Decorators;
using Intent.Modules.WebApi.Legacy.Controller;
using Intent.Modules.WebApi.Templates.HttpExceptionHandler;
using Intent.Modules.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.WebApi.Templates.WebApiBadHttpRequestException;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Modules.Decorators.WebApi;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Auditing
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

            RegisterTemplate(HttpRequestMessageExtensionsTemplate.Identifier, project => new HttpRequestMessageExtensionsTemplate(project));
            RegisterTemplate(ServiceBoundaryAuditingStrategyTemplate.Identifier, project => new ServiceBoundaryAuditingStrategyTemplate(project));

            RegisterDecorator<IDistributionDecorator>(AuditingDistributionDecorator.Identifier, new AuditingDistributionDecorator(application));
            RegisterDecorator<IUnityRegistrationsDecorator>(AuditingUnityRegistrationsDecorator.Identifier, new AuditingUnityRegistrationsDecorator());
        }
    }
}
