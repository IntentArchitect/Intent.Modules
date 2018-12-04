using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.WebApi.Interop.Autofac.Decorators
{
    public class AutofacWebApiConfigTemplateDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => AutofacWebApiConfigTemplateDecorator.Id;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new AutofacWebApiConfigTemplateDecorator(application);
        }
    }
}
