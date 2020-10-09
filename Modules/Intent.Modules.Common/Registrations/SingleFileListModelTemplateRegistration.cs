using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    public abstract class SingleFileListModelTemplateRegistration<TModel> : TemplateRegistrationBase
        where TModel: class
    {
        public abstract TModel GetModels(IApplication application);

        public abstract ITemplate CreateTemplateInstance(IOutputTarget project, TModel model);

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, context => CreateTemplateInstance(context, GetModels(application)));
        }
    }
}