using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Interop.Unity.Decorators;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.EntityFramework.Interop.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(EntityFrameworkUnityRegistrationsDecorator.Identifier, new EntityFrameworkUnityRegistrationsDecorator(application));
        }
    }
}
