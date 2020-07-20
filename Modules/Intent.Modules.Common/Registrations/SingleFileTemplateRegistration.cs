using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    public abstract class SingleFileTemplateRegistration : TemplateRegistrationBase
    {
        public abstract ITemplate CreateTemplateInstance(IOutputContext project);

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, CreateTemplateInstance);
        }
    }

    public abstract class SingleFileRegistrationBase<TModel> : TemplateRegistrationBase
        where TModel: class
    {
        public abstract TModel GetModels(IApplication application);

        public abstract ITemplate CreateTemplateInstance(IOutputContext project, TModel model);

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, context => CreateTemplateInstance(context, GetModels(application)));
        }
    }
}