using System.ComponentModel;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Eventing;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Interop.WebApi.Decorators
{
    [Description(WebApiControllerDecorator.Identifier)]
    public class WebApiControllerDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => WebApiControllerDecorator.Identifier;
        public override WebApiControllerDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new WebApiControllerDecorator(application);
        }
    }
}
