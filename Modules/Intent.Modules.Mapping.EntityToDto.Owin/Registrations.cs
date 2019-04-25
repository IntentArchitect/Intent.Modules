using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Mapping.EntityToDto.Owin.Decorators;
using Intent.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Mapping.EntityToDto.Owin
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(AutomapperOwinStartupDecorator.IDENTIFIER, new AutomapperOwinStartupDecorator());
        }
    }
}
