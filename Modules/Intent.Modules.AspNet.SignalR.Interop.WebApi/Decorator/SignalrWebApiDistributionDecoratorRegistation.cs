using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator
{
    public class SignalrWebApiDistributionDecoratorRegistation : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => SignalrWebApiWebApiControllerDecorator.Identifier;
        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SignalrWebApiWebApiControllerDecorator();
        }
    }
}
