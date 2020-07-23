using System.Collections.Generic;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    public abstract class FilePerModelTemplateRegistration<TModel> : TemplateRegistrationBase
    {
        public abstract ITemplate CreateTemplateInstance(ITemplateExecutionContext outputContext, TModel model);
        public abstract IEnumerable<TModel> GetModels(IApplication application);

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            foreach (var model in GetModels(application))
            {
                registry.RegisterTemplate(TemplateId, context => CreateTemplateInstance(context, model));
            }
        }
    }
}