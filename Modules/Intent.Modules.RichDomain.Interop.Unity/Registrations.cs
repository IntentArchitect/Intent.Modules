using Intent.Modules.Common.Registrations;
using Intent.Modules.RichDomain.Interop.Unity.Decorators;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.RichDomain.Interop.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(DomainUnityConfigurationDecorator.Identifier, new DomainUnityConfigurationDecorator());
        }
    }
}
