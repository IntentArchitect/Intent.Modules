using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Common.Registrations
{
    public abstract class NoModelTemplateRegistrationBase : IProjectTemplateRegistration
    {
        public abstract string TemplateId { get; }

        public abstract ITemplate CreateTemplateInstance(IProject project);

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        {
            registery.Register(TemplateId, project => CreateTemplateInstance(project));
        }
    }
}
