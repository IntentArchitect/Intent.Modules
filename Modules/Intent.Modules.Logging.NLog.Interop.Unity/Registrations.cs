using Intent.Packages.Logging.NLog.Interop.Unity.Decorators;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Logging.NLog.Interop.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(NLogUnityDecorator.Identifier, new NLogUnityDecorator(application));
        }
    }
}
