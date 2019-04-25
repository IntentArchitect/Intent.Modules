using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

using System;

namespace Intent.Modules.IdentityServer.Templates.Scopes
{
    public class ScopesRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => IdentityServerScopesTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new IdentityServerScopesTemplate(project));
        }
    }
}
