using Intent.Engine;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.Owin.Cors.Decorators
{
    public class CorsOwinStartupDecoratorRegistration : DecoratorRegistration<IOwinStartupDecorator>
    {
        public override string DecoratorId => CorsOwinStartupDecorator.Identifier;

        public override IOwinStartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new CorsOwinStartupDecorator();
        }
    }
}
