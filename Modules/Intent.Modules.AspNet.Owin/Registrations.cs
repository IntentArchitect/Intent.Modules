using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.Owin
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(OwinStartupTemplate.Identifier, project => new OwinStartupTemplate(project));
        }
    }
}
