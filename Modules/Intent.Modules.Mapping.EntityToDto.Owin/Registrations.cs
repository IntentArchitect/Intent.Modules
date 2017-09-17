using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Mapping.EntityToDto.Owin.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Mapping.EntityToDto.Owin
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
