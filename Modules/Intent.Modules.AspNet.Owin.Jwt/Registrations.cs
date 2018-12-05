using Intent.Modules.AspNet.Owin.Jwt.Decorators;
using Intent.Modules.AspNet.Owin.Jwt.Templates.SigningCertificate;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.AspNet.Owin.Jwt
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IOwinStartupDecorator>(JwtAuthOwinStartupDecorator.Identifier, new JwtAuthOwinStartupDecorator(application.SolutionEventDispatcher));

            RegisterTemplate(SigningCertificateTemplate.Identifier, project => new SigningCertificateTemplate(project));
        }
    }
}
