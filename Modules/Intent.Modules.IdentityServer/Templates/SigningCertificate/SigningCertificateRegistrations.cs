using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

using System;

namespace Intent.Modules.IdentityServer.Templates.SigningCertificate
{
    public class SigningCertificateRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => SigningCertificateTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new SigningCertificateTemplate(project));
        }
    }
}
