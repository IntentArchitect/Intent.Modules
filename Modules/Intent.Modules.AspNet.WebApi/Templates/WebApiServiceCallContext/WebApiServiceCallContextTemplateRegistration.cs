using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.AspNet.WebApi.Templates.WebApiServiceCallContext
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
