using Intent.Packages.UserContext.Interop.WebApi.Decorators;
using Intent.Modules.WebApi.Legacy.Controller;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.UserContext.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IDistributionDecorator>(Legacy.UserContextDistributionDecorator.Identifier, new Legacy.UserContextDistributionDecorator());
            RegisterDecorator<DistributionDecoratorBase>(UserContextDistributionDecorator.Identifier, new UserContextDistributionDecorator());
        }
    }
}
