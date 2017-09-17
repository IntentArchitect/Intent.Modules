using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.Modules.Auditing.Decorators;
using Intent.Modules.Auditing.Templates.HttpRequestMessageExtensions;
using Intent.Modules.Auditing.Templates.ServiceBoundaryAudtingStrategy;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.Auditing
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
