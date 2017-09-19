using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator
{
    public class SignalrWebApiDistributionDecoratorRegistation : DecoratorRegistration<DistributionDecoratorBase>
    {
        public override string DecoratorId => SignalrWebApiDistributionDecorator.Identifier;
        public override object CreateDecoratorInstance()
        {
            return new SignalrWebApiDistributionDecorator();
        }
    }
}
