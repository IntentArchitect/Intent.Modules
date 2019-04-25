using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    public abstract class NoModelTemplateRegistrationBase : IProjectTemplateRegistration
    {
        public abstract string TemplateId { get; }

        public abstract ITemplate CreateTemplateInstance(IProject project);

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.Register(TemplateId, CreateTemplateInstance);
        }
    }
}
