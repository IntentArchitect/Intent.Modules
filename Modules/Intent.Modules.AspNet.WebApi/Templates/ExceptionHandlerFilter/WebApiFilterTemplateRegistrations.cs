using Intent.Engine;
using Intent.Templates;

using System;
using Intent.Modules.Common.Registrations;

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
