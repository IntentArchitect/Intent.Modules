using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.AspNet.WebApi.Decorators;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.AspNet.WebApi.Templates.HttpExceptionHandler;
using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.AspNet.WebApi.Templates.RequireHttpsMiddleware;
using Intent.Modules.AspNet.WebApi.Templates.WebApiBadHttpRequestException;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.AspNet.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(HttpExceptionHandlerTemplate.Identifier, project => new HttpExceptionHandlerTemplate(project, application.EventDispatcher));
            RegisterTemplate(WebApiBadHttpRequestExceptionTemplate.Identifier, project => new WebApiBadHttpRequestExceptionTemplate(project));
            RegisterTemplate(RequireHttpsMiddlewareTemplate.Identifier, project => new RequireHttpsMiddlewareTemplate(project));

            RegisterDecorator<WebApiControllerDecoratorBase>(WebApiWebApiControllerExceptionHandlingStrategy.Identifier, new WebApiWebApiControllerExceptionHandlingStrategy());
            RegisterDecorator<WebApiConfigTemplateDecoratorBase>(WebApiConfigJsonValidationDecorator.Identifier, new WebApiConfigJsonValidationDecorator());
            RegisterDecorator<IOwinStartupDecorator>(WebApiOwinStartupDecorator.Identifier, new WebApiOwinStartupDecorator());
            RegisterDecorator<IOwinStartupDecorator>(UseHttpsOwinStartupDecorator.Identifier, new UseHttpsOwinStartupDecorator());
        }
    }
}
