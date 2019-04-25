using System.Collections.Generic;
using System.ComponentModel;
using Intent.MetaModel.Service;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates


namespace Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig
{
    [Description(OwinWebApiConfigTemplate.Identifier)]
    public class OwinWebApiConfigTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => OwinWebApiConfigTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new OwinWebApiConfigTemplate(project);
        }
    }
}

