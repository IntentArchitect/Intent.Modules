using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity.Decorators
{
    public class UnityWebApiConfigTemplateDecoratorRegistration : DecoratorRegistration<IWebApiConfigTemplateDecorator>
    {
        public override string DecoratorId => UnityWebApiConfigTemplateDecorator.Id;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new UnityWebApiConfigTemplateDecorator();
        }
    }
}
