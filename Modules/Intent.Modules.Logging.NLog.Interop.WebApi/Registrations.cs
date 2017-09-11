using Intent.Packages.Logging.NLog.Interop.WebApi.Decorators;
using Intent.Modules.WebApi.Legacy.Controller;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Logging.NLog.Interop.WebApi
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
