using System.Linq;
using Intent.Packages.Messaging.Publisher.Decorators.Legacy;
using Intent.Packages.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.Modules.WebApi.Legacy.Controller;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.VSProjects.Decorators;
using IntentEsbPublishingDistributionDecorator = Intent.Packages.Messaging.Publisher.Decorators.IntentEsbPublishingDistributionDecorator;

namespace Intent.Packages.Messaging.Publisher
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<DistributionDecoratorBase>(IntentEsbPublishingDistributionDecorator.Identifier, new IntentEsbPublishingDistributionDecorator());
            RegisterDecorator<IWebConfigDecorator>(IntentEsbPublishingWebConfigDecorator.Identifier, new IntentEsbPublishingWebConfigDecorator());

            RegisterDecorator<IEventConsumerDecorator>(Decorators.Legacy.IntentEsbPublishingEventConsumerDecorator.Identifier, new Decorators.Legacy.IntentEsbPublishingEventConsumerDecorator());
            RegisterDecorator<IDistributionDecorator>(Decorators.Legacy.IntentEsbPublishingDistributionDecorator.Identifier, new Decorators.Legacy.IntentEsbPublishingDistributionDecorator());

            var applicationModel = metaDataManager.GetMetaData<ApplicationModel>(new MetaDataType("Application")).FirstOrDefault(x => x.Name == application.ApplicationName);
            if (applicationModel == null)
            {
                Logging.Log.Warning($"ApplicationModel could not be found for application [{ application.ApplicationName }]");
                return;
            }

            RegisterDecorator<IUnityRegistrationsDecorator>(IntentEsbPublishingUnityRegistrationsDecorator.Identifier, new IntentEsbPublishingUnityRegistrationsDecorator(application, applicationModel.EventingModel));
        }
    }
}
