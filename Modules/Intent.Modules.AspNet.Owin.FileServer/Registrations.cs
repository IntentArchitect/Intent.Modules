using Intent.Modules.AspNet.Owin.FileServer.Decorators;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.AspNet.Owin.FileServer
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metadataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(RootLocationOwinStartupDecorator.Identifier, new RootLocationOwinStartupDecorator());
            RegisterDecorator<IWebConfigDecorator>(RootLocationWebConfigDecorator.Identifier, new RootLocationWebConfigDecorator());
        }
    }
}
