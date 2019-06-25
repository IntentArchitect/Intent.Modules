using Intent.Modules.AspNet.Owin.Cors.Decorators;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.AspNet.Owin.Cors
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metadataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(CorsOwinStartupDecorator.Identifier, new CorsOwinStartupDecorator());
        }
    }
}
