using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class WebApiConfigJsonValidationDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => WebApiConfigJsonValidationDecorator.Identifier;

        public override WebApiConfigTemplateDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new WebApiConfigJsonValidationDecorator();
        }
    }
}
