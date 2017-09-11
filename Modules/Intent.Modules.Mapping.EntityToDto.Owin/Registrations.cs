using Intent.Packages.Mapping.EntityToDto.Owin.Decorators;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Mapping.EntityToDto.Owin
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(AutomapperOwinStartupDecorator.Identifier, new AutomapperOwinStartupDecorator());
        }
    }
}
