using System.ComponentModel;
using Intent.Modules.AspNetCore.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.EntityFrameworkCore.Interop.WebApi.Decorators
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
