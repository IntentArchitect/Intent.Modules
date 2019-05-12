using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Engine;
using Intent.Registrations;
using System;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity.Decorators
{
    public class UnityWebApiConfigTemplateDecoratorRegistration : DecoratorRegistration<WebApiConfigTemplateDecoratorBase>
    {
        public override string DecoratorId => UnityWebApiConfigTemplateDecorator.Id;
        public override WebApiConfigTemplateDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new UnityWebApiConfigTemplateDecorator(application);
        }
    }
}
