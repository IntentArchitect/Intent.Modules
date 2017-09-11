using Intent.Packages.EntityFramework.Interop.Unity.Decorators;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.EntityFramework.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(EntityFrameworkUnityRegistrationsDecorator.Identifier, new EntityFrameworkUnityRegistrationsDecorator(application));
        }
    }
}
