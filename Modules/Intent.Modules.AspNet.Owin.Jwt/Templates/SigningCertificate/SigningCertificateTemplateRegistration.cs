using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNet.Owin.Jwt.Templates.SigningCertificate
{
    [Description(SigningCertificateTemplate.Identifier)]
    public class SigningCertificateTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => SigningCertificateTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new SigningCertificateTemplate(project);
        }
    }
}
