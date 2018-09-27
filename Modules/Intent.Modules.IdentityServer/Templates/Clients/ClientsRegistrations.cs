using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;
using System;

namespace Intent.Modules.IdentityServer.Templates.Clients
{
    public class ClientsRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => IdentityServerClientsTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new IdentityServerClientsTemplate(project, applicationManager.SolutionEventDispatcher));
        }
    }
}
