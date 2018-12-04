using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.Swashbuckle.Decorators
{
    public class SwashbuckleWebApiConfigTemplateDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => SwashbuckleWebApiConfigTemplateDecorator.Id;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new SwashbuckleWebApiConfigTemplateDecorator(application);
        }
    }
}
