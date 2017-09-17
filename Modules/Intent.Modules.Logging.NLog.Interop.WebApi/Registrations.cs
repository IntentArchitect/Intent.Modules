using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Logging.NLog.Interop.WebApi.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Logging.NLog.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IDistributionDecorator>(Legacy.NLogDistributionDecorator.Identifier, new Legacy.NLogDistributionDecorator());
            RegisterDecorator<DistributionDecoratorBase>(NLogDistributionDecorator.Identifier, new NLogDistributionDecorator());
        }
    }
}
