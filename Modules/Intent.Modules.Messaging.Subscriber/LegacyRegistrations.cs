using Intent.Modules.Messaging.Subscriber.Legacy.Decorators;
using Intent.Modules.Messaging.Subscriber.Legacy.MessageHandler;
using Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.Messaging.Subscriber
{
    public class LegacyRegistrations : OldProjectTemplateRegistration
    {

        public LegacyRegistrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var applicationModel = metaDataManager.GetMetaData<ApplicationModel>(new MetaDataType("Application")).FirstOrDefault(x => x.Name == application.ApplicationName);
            if (applicationModel == null)
            {
                Logging.Log.Warning($"ApplicationModel could not be found for application [{ application.ApplicationName }]");
                return;
            }

            RegisterDecorator<IUnityRegistrationsDecorator>(IntentEsbConsumingUnityRegistrationsDecorator.Identifier, new IntentEsbConsumingUnityRegistrationsDecorator(application, applicationModel.EventingModel));

            var subscribing = applicationModel.EventingModel.Subscribing;
            if (subscribing.ConsumptionChannel == EventConsumptionChannel.WebApi)
            {
                RegisterTemplate(WebApiEventConsumerServiceTemplate.Identifier, project => new WebApiEventConsumerServiceTemplate(project, applicationModel.EventingModel.Subscribing, application.EventDispatcher));
            }
            foreach (var eventType in subscribing.SubscribedEvents)
            {
                RegisterTemplate(MessageHandlerTemplate.Identifier, project => new MessageHandlerTemplate(project, eventType));
            }
        }
    }
}
