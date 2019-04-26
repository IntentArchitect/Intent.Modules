using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;


namespace Intent.Modules.AspNet.WebApi.Templates.WebApiServiceCallContext
{
    [Description(WebApiServiceCallContextTemplate.IDENTIFIER)]
    public class WebApiServiceCallContextTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => WebApiServiceCallContextTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new WebApiServiceCallContextTemplate(project);
        }
    }
}
