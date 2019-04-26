using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.AspNet.Owin
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterTemplate(OwinStartupTemplate.Identifier, project => new OwinStartupTemplate(project));
        }
    }
}
