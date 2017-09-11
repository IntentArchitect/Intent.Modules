using Intent.Packages.Owin.FileServer.Decorators;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.VSProjects.Decorators;

namespace Intent.Packages.Owin.FileServer
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(RootLocationOwinStartupDecorator.Identifier, new RootLocationOwinStartupDecorator());
            RegisterDecorator<IWebConfigDecorator>(RootLocationWebConfigDecorator.Identifier, new RootLocationWebConfigDecorator());
        }
    }
}
