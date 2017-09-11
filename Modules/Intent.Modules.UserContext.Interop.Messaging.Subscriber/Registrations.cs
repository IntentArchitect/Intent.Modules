using Intent.Packages.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.Packages.UserContext.Interop.Messaging.Subscriber.Legacy;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.UserContext.Interop.Messaging.Subscriber
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IEventConsumerDecorator>(UserContextEventConsumerDecorator.Identifier, new UserContextEventConsumerDecorator());
        }
    }
}
