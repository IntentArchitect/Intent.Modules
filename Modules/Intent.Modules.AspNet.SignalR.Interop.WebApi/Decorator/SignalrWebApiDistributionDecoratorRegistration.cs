using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator
{
    public class SignalrWebApiDistributionDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => SignalrWebApiWebApiControllerDecorator.Identifier;
        public override WebApiControllerDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new SignalrWebApiWebApiControllerDecorator();
        }
    }
}
