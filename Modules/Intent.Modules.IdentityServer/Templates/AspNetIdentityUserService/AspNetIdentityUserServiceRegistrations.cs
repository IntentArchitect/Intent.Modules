using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;
using System;

namespace Intent.Modules.IdentityServer.Templates.AspNetIdentityUserService
{
    public class AspNetIdentityUserServiceRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => AspNetIdentityUserServiceTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new AspNetIdentityUserServiceTemplate(project));
        }
    }
}
