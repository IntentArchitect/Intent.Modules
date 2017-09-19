using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator.Legacy
{
    public class LegacySignalrWebApiDistributionDecoratorRegistation : DecoratorRegistration<IDistributionDecorator>
    {
        public override string DecoratorId => LegacySignalrWebApiDistributionDecorator.Identifier;
        public override object CreateDecoratorInstance()
        {
            return new LegacySignalrWebApiDistributionDecorator();
        }
    }
}
