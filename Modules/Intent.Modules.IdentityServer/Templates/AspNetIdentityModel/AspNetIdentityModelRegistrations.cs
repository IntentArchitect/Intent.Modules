using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

using System;

namespace Intent.Modules.IdentityServer.Templates.AspNetIdentityModel
{
    public class AspNetIdentityModelRegistrations : IProjectTemplateRegistration
    {
        public string TemplateId => AspNetIdentityModelTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new AspNetIdentityModelTemplate(project, applicationManager.EventDispatcher));
        }
    }
}
