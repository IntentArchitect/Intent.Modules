using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.AspNet.WebApi.Decorators;
using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.AspNet.WebApi.Templates.RequireHttpsMiddleware;
using Intent.Engine;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterTemplate(RequireHttpsMiddlewareTemplate.Identifier, project => new RequireHttpsMiddlewareTemplate(project));

            RegisterDecorator<WebApiConfigTemplateDecoratorBase>(WebApiConfigJsonValidationDecorator.Identifier, new WebApiConfigJsonValidationDecorator());
            RegisterDecorator<IOwinStartupDecorator>(WebApiOwinStartupDecorator.Identifier, new WebApiOwinStartupDecorator());
            RegisterDecorator<IOwinStartupDecorator>(UseHttpsOwinStartupDecorator.Identifier, new UseHttpsOwinStartupDecorator());
        }
    }
}
