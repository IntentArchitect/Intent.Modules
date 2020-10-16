using Intent.Engine;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.Owin.Jwt.Decorators
{
    public class JwtAuthOwinStartupDecoratorRegistration : DecoratorRegistration<IOwinStartupDecorator>
    {
        public override string DecoratorId => JwtAuthOwinStartupDecorator.Identifier;

        public override IOwinStartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new JwtAuthOwinStartupDecorator(application.EventDispatcher);
        }
    }
}
