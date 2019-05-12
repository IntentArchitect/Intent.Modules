using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Engine;
using Intent.Registrations;
using System;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNet.WebApi.Interop.Autofac.Decorators
{
    public class AutofacWebApiConfigTemplateDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => AutofacWebApiConfigTemplateDecorator.Id;
        public override WebApiConfigTemplateDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new AutofacWebApiConfigTemplateDecorator(application);
        }
    }
}
