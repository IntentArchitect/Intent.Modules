using Intent.Modules.RichDomain.Interop.Unity.Decorators;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.RichDomain.Interop.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(DomainUnityConfigurationDecorator.Identifier, new DomainUnityConfigurationDecorator());
        }
    }
}
