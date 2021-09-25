using System.Collections.Generic;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    /// <summary>
    /// Template Registration that produces a file per module that is returned by the <see cref="GetModels"/> method.
    /// <para>
    /// Learn more about template registrations in
    /// <seealso href="https://intentarchitect.com/docs/articles/references/templates-csharp/templates-csharp.html#3-template-registration-file">this article</seealso>.
    /// </para>
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class FilePerModelTemplateRegistration<TModel> : TemplateRegistrationBase
    {
        /// <summary>
        /// Returns the template instance. This method is run for each <typeparamref name="TModel"/> <paramref name="model"/> that is
        /// returned from the <see cref="GetModels"/> method.
        /// </summary>
        /// <param name="outputTarget"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract ITemplate CreateTemplateInstance(IOutputTarget outputTarget, TModel model);

        /// <summary>
        /// Implement to determine which instances of <typeparamref name="TModel"/> must create a file based on the template.
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
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