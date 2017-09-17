using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.UserContext.Interop.AspNet.WebApi.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.UserContext.Interop.AspNet.WebApi
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
