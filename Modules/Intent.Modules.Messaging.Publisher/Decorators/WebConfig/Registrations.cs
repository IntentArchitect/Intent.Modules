using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.Messaging.Publisher.Decorators.WebConfig
{
    [Description(WebConfigDecorator.IDENTIFIER)]
    public class Registrations : DecoratorRegistration<IWebConfigDecorator>
    {
        public override string DecoratorId => WebConfigDecorator.IDENTIFIER;
        public override object CreateDecoratorInstance(IApplication application)
        {
            return new WebConfigDecorator();
        }
    }
}
