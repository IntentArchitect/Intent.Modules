using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Messaging.Publisher.Decorators.Legacy;
using Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Registrations;
using System.Linq;
using IntentEsbPublishingDistributionDecorator = Intent.Modules.Messaging.Publisher.Decorators.IntentEsbPublishingDistributionDecorator;

namespace Intent.Modules.Messaging.Publisher
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

            var applicationModel = metaDataManager.GetMetaData<ApplicationModel>(new MetaDataIdentifier("Application")).FirstOrDefault(x => x.Name == application.ApplicationName);
            if (applicationModel == null)
            {
                Logging.Log.Warning($"ApplicationModel could not be found for application [{ application.ApplicationName }]");
                return;
            }

            RegisterDecorator<IUnityRegistrationsDecorator>(IntentEsbPublishingUnityRegistrationsDecorator.Identifier, new IntentEsbPublishingUnityRegistrationsDecorator(application, applicationModel.EventingModel));
        }
    }
}
