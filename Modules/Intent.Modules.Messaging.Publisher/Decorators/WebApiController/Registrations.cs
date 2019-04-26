using System.ComponentModel;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.Messaging.Publisher.Decorators.WebApiController
{
    [Description(WebApiControllerDecorator.IDENTIFIER)]
    public class Registrations : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => WebApiControllerDecorator.IDENTIFIER;
        public override object CreateDecoratorInstance(IApplication application)
        {
            return new WebApiControllerDecorator();
        }
    }
}
