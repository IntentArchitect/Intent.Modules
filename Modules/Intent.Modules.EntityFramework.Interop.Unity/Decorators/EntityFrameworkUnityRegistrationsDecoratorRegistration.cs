using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Unity.Templates.UnityConfig;

namespace Intent.Modules.EntityFramework.Interop.Unity.Decorators
{
    public class EntityFrameworkUnityRegistrationsDecoratorRegistration : DecoratorRegistration<IUnityRegistrationsDecorator>
    {
        public override string DecoratorId => EntityFrameworkUnityRegistrationsDecorator.Identifier;

        public override IUnityRegistrationsDecorator CreateDecoratorInstance(IApplication application)
        {
            return new EntityFrameworkUnityRegistrationsDecorator(application);
        }
    }
}
