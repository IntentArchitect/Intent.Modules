using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    public abstract class SingleFileTemplateRegistration : TemplateRegistrationBase
    {
        public abstract ITemplate CreateTemplateInstance(IOutputTarget project);

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, CreateTemplateInstance);
        }
    }
}