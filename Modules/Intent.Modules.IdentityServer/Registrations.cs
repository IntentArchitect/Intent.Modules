using Intent.MetaModel.Hosting;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.IdentityServer.Decorators;
using Intent.Modules.IdentityServer.Templates.AspNetIdentityModel;
using Intent.Modules.IdentityServer.Templates.AspNetIdentityUserService;
using Intent.Modules.IdentityServer.Templates.Clients;
using Intent.Modules.IdentityServer.Templates.Scopes;
using Intent.Modules.IdentityServer.Templates.SigningCertificate;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.IdentityServer
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var hostingConfig = metaDataManager.GetMetaData<HostingConfigModel>("LocalHosting").SingleOrDefault(x => x.ApplicationName == application.ApplicationName);

            RegisterDecorator<IOwinStartupDecorator>(IdentityServerOwinStartupDecorator.Identifier, new IdentityServerOwinStartupDecorator(hostingConfig, application.EventDispatcher, application.SolutionEventDispatcher));

            RegisterTemplate(SigningCertificateTemplate.Identifier, project => new SigningCertificateTemplate(project));
            RegisterTemplate(AspNetIdentityModelTemplate.Identifier, project => new AspNetIdentityModelTemplate(project, application.EventDispatcher));
            RegisterTemplate(AspNetIdentityUserServiceTemplate.Identifier, project => new AspNetIdentityUserServiceTemplate(project));
            RegisterTemplate(IdentityServerClientsTemplate.Identifier, project => new IdentityServerClientsTemplate(project, application.SolutionEventDispatcher));
            RegisterTemplate(IdentityServerScopesTemplate.Identifier, project => new IdentityServerScopesTemplate(project));
        }
    }
}
