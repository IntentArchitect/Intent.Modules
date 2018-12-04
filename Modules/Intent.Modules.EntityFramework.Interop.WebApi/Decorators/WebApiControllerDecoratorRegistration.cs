using System.ComponentModel;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.EntityFramework.Interop.WebApi.Decorators
{
    [Description(WebApiControllerDecorator.Identifier)]
    public class WebApiControllerDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public WebApiControllerDecoratorRegistration()
        {
            
        }

        public override string DecoratorId => WebApiControllerDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new WebApiControllerDecorator(application);
        }
    }
}
