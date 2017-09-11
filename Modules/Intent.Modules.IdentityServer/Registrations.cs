using System.Linq;
using Intent.MetaModel.Hosting;
using Intent.Packages.IdentityServer.Decorators;
using Intent.Packages.IdentityServer.Templates.AspNetIdentityModel;
using Intent.Packages.IdentityServer.Templates.AspNetIdentityUserService;
using Intent.Packages.IdentityServer.Templates.Clients;
using Intent.Packages.IdentityServer.Templates.Scopes;
using Intent.Packages.IdentityServer.Templates.SigningCertificate;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.IdentityServer
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
