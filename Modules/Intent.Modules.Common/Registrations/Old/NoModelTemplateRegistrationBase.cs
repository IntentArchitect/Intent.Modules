using System;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    [Obsolete("Use SingleFileTemplateRegistration")]
    public abstract class NoModelTemplateRegistrationBase : ITemplateRegistration
    {
        public abstract string TemplateId { get; }

        public abstract ITemplate CreateTemplateInstance(IProject project);

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, context => CreateTemplateInstance((IProject)context));
        }
    }
}
