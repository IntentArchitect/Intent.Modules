using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Packages.Owin.Jwt.Decorators;
using Intent.Packages.Owin.Jwt.Templates.SigningCertificate;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Owin
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
