using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class WebApiOwinStartupDecoratorRegistration : DecoratorRegistration<IOwinStartupDecorator>
    {
        public override string DecoratorId => WebApiOwinStartupDecorator.Identifier;

        public override IOwinStartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new WebApiOwinStartupDecorator();
        }
    }
}
