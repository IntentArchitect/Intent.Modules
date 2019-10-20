using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Decorators;

namespace Intent.Modules.AspNet.Owin.FileServer.Decorators
{
    public class RootLocationWebConfigDecoratorRegistration : DecoratorRegistration<IWebConfigDecorator>
    {
        public override string DecoratorId => RootLocationWebConfigDecorator.Identifier;

        public override IWebConfigDecorator CreateDecoratorInstance(IApplication application)
        {
            return new RootLocationWebConfigDecorator();
        }
    }
}
