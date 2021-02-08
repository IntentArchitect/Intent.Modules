using System.Collections.Generic;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    /// <summary>
    /// Template Registration that produces as single file. Passes in a list of models into the template.
    /// </summary>
    public abstract class SingleFileListModelTemplateRegistration<TModel> : TemplateRegistrationBase
        where TModel: class
    {
        private IList<TModel> _models;

        public abstract IList<TModel> GetModels(IApplication application);

        public abstract ITemplate CreateTemplateInstance(IOutputTarget project, IList<TModel> model);

        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            _models = GetModels(application);
            base.DoRegistration(registry, application);
        }

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, context => CreateTemplateInstance(context, _models));
        }
    }
}