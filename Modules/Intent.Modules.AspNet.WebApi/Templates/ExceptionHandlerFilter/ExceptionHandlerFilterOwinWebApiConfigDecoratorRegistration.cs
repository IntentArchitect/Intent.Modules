using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;

namespace Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter
{
    public class ExceptionHandlerFilterOwinWebApiConfigDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => ExceptionHandlerFilterOwinWebApiConfigDecorator.DecoratorId;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new ExceptionHandlerFilterOwinWebApiConfigDecorator();
        }
    }
}
