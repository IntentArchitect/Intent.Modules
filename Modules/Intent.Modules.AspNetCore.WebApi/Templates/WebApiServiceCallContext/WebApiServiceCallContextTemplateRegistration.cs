using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;


namespace Intent.Modules.AspNetCore.WebApi.Templates.WebApiServiceCallContext
{
    [Description(WebApiServiceCallContextTemplate.Identifier)]
    public class WebApiServiceCallContextTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => WebApiServiceCallContextTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new WebApiServiceCallContextTemplate(project);
        }
    }
}
