using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup;
using Intent.SoftwareFactory.Engine;
using System;

namespace Intent.Modules.IdentityServer4.Decorators
{
    public class IdentityServer4CoreWebStartupDecoratorRegistration : DecoratorRegistration<CoreWebStartupDecorator>
    {
        public override string DecoratorId => IdentityServer4CoreWebStartupDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new IdentityServer4CoreWebStartupDecorator();
        }
    }
}
