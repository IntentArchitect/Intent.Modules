using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.EntityFramework.Interop.WebApi.Legacy;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.EntityFramework.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IDistributionDecorator>(EntityFrameworkDistributionDecorator.Identifier, new EntityFrameworkDistributionDecorator());
            RegisterDecorator<DistributionDecoratorBase>(Decorators.EntityFrameworkDistributionDecorator.Identifier, new Decorators.EntityFrameworkDistributionDecorator());
        }
    }
}
