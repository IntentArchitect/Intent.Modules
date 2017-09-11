using Intent.Packages.Owin.Cors.Decorators;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Owin.Cors
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(CorsOwinStartupDecorator.Identifier, new CorsOwinStartupDecorator());
        }
    }
}
