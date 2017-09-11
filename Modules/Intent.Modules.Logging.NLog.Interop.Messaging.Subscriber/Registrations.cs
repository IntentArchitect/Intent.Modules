using Intent.Packages.Logging.NLog.Interop.Messaging.Subscriber.Legacy;
using Intent.Packages.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Logging.NLog.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IEventConsumerDecorator>(NLogEventConsumerDecorator.Identifier, new NLogEventConsumerDecorator());
        }
    }
}
