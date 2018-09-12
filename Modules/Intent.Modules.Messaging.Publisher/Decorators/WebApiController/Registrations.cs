using System.ComponentModel;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

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
