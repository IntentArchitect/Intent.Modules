using Intent.Modules.Logging.NLog.Interop.Messaging.Subscriber.Legacy;
using Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Logging.NLog.Interop.Messaging.Subscriber
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IEventConsumerDecorator>(NLogEventConsumerDecorator.Identifier, new NLogEventConsumerDecorator());
        }
    }
}
