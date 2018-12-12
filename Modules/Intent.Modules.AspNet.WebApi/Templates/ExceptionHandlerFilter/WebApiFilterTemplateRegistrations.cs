using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;

namespace Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter
{
    public class WebApiFilterTemplateRegistrations : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => WebApiFilterTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new WebApiFilterTemplate(TemplateId, project);
        }
    }
}
