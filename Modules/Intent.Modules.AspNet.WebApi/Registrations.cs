using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.AspNet.WebApi.Decorators;
using Intent.Modules.AspNet.WebApi.Legacy.Controller;
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
using WebApiControllerTemplate = Intent.Modules.AspNet.WebApi.Legacy.Controller.WebApiControllerTemplate;

namespace Intent.Modules.AspNet.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

            RegisterTemplate(OwinWebApiConfigTemplate.Identifier, project => new OwinWebApiConfigTemplate(project));
            RegisterTemplate(HttpExceptionHandlerTemplate.Identifier, project => new HttpExceptionHandlerTemplate(project, application.EventDispatcher));
            RegisterTemplate(WebApiBadHttpRequestExceptionTemplate.Identifier, project => new WebApiBadHttpRequestExceptionTemplate(project));
            RegisterTemplate(RequireHttpsMiddlewareTemplate.Identifier, project => new RequireHttpsMiddlewareTemplate(project));

            foreach (var serviceModel in serviceModels)
            {
                RegisterTemplate(WebApiControllerTemplate.Identifier, project => new WebApiControllerTemplate(serviceModel, project));
            }

            RegisterDecorator<DistributionDecoratorBase>(WebApiDistributionExceptionHandlingStrategy.Identifier, new WebApiDistributionExceptionHandlingStrategy());
            RegisterDecorator<IDistributionDecorator>(Decorators.Legacy.WebApiDistributionExceptionHandlingStrategy.Identifier, new Decorators.Legacy.WebApiDistributionExceptionHandlingStrategy());
            RegisterDecorator<IDistributionDecorator>(Decorators.Legacy.WebApiDistributionJsonValidationDecorator.Identifier, new Decorators.Legacy.WebApiDistributionJsonValidationDecorator());
            RegisterDecorator<IDistributionDecorator>(Decorators.Legacy.DebugSleepDistributionDecorator.Identifier, new Decorators.Legacy.DebugSleepDistributionDecorator(300));
            RegisterDecorator<IWebApiConfigTemplateDecorator>(WebApiConfigJsonValidationDecorator.Identifier, new WebApiConfigJsonValidationDecorator());
            RegisterDecorator<IOwinStartupDecorator>(WebApiOwinStartupDecorator.Identifier, new WebApiOwinStartupDecorator());
            RegisterDecorator<IOwinStartupDecorator>(UseHttpsOwinStartupDecorator.Identifier, new UseHttpsOwinStartupDecorator());
        }
    }
}
