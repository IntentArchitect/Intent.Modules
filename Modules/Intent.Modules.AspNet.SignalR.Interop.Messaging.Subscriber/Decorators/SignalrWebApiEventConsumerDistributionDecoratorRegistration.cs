using Intent.Packages.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.AspNet.SignalR.Interop.Messaging.Subscriber.Decorators
{
    public class SignalrWebApiEventConsumerDistributionDecoratorRegistration : DecoratorRegistration<IEventConsumerDecorator>
    {
        public override string DecoratorId => SignalrWebApiEventConsumerDistributionDecorator.Identifier;
        public override object CreateDecoratorInstance()
        {
            return new SignalrWebApiEventConsumerDistributionDecorator();
        }
    }
}
