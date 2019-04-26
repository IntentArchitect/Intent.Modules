using Intent.Modules.Common.Registrations;
using Intent.Modules.Logging.NLog.Interop.Unity.Decorators;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.Logging.NLog.Interop.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterDecorator<IUnityRegistrationsDecorator>(NLogUnityDecorator.Identifier, new NLogUnityDecorator(application));
        }
    }
}
