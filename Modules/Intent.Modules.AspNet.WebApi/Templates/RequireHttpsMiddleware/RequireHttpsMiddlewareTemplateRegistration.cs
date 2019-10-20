using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.AspNet.WebApi.Templates.RequireHttpsMiddleware
{
    [Description(RequireHttpsMiddlewareTemplate.Identifier)]
    public class RequireHttpsMiddlewareTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => RequireHttpsMiddlewareTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new RequireHttpsMiddlewareTemplate(project);
        }
    }
}
