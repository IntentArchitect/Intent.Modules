using System.ComponentModel;
using Intent.Modules.AspNetCore.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.Engine;

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
