using Intent.Packages.RichDomain.Interop.Unity.Decorators;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.RichDomain.Interop.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(DomainUnityConfigurationDecorator.Identifier, new DomainUnityConfigurationDecorator());
        }
    }
}
