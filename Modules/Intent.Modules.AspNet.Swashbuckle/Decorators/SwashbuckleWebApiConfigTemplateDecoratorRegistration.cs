using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNet.Swashbuckle.Decorators
{
    public class SwashbuckleWebApiConfigTemplateDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => SwashbuckleWebApiConfigTemplateDecorator.Id;
        public override WebApiConfigTemplateDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new SwashbuckleWebApiConfigTemplateDecorator(application);
        }
    }
}
