using Intent.Packages.EntityFramework.Interop.WebApi.Decorators;
using Intent.Modules.WebApi.Legacy.Controller;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.EntityFramework.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IDistributionDecorator>(Intent.Packages.EntityFramework.Interop.WebApi.Legacy.EntityFrameworkDistributionDecorator.Identifier, new Intent.Packages.EntityFramework.Interop.WebApi.Legacy.EntityFrameworkDistributionDecorator());
            RegisterDecorator<DistributionDecoratorBase>(EntityFrameworkDistributionDecorator.Identifier, new EntityFrameworkDistributionDecorator());
        }
    }
}
