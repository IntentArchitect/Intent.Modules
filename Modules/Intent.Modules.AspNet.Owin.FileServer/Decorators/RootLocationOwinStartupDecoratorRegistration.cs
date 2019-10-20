using Intent.Engine;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.Owin.FileServer.Decorators
{
    public class RootLocationOwinStartupDecoratorRegistration : DecoratorRegistration<IOwinStartupDecorator>
    {
        public override string DecoratorId => RootLocationOwinStartupDecorator.Identifier;

        public override IOwinStartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new RootLocationOwinStartupDecorator();
        }
    }
}
