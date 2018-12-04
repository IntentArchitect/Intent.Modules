using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

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
