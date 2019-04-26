using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    [Description(SwashbuckleCoreWebStartupDecorator.Identifier)]
    public class SwashbuckleCoreWebStartupDecoratorRegistration : DecoratorRegistration<CoreWebStartupDecorator>
    {
        public override string DecoratorId => SwashbuckleCoreWebStartupDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SwashbuckleCoreWebStartupDecorator();
        }
    }
}
