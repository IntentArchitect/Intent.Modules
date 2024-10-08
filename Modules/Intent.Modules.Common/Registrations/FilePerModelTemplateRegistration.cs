﻿using System.Collections.Generic;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    /// <summary>
    /// Template Registration that produces a file per module that is returned by the <see cref="GetModels"/> method.
    /// <para>
    /// Learn more about template registrations in
    /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=templateRegistrations">
    /// this article</seealso>.
    /// </para>
    /// </summary>
    public abstract class FilePerModelTemplateRegistration<TModel> : TemplateRegistrationBase
    {
        /// <summary>
        /// Returns the template instance. This method is run for each <typeparamref name="TModel"/> <paramref name="model"/> that is
        /// returned from the <see cref="GetModels"/> method.
        /// </summary>
        public abstract ITemplate CreateTemplateInstance(IOutputTarget outputTarget, TModel model);

        /// <summary>
        /// Implement to determine which instances of <typeparamref name="TModel"/> must create a file based on the template.
        /// </summary>
        public abstract IEnumerable<TModel> GetModels(IApplication application);

        /// <inheritdoc />
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            foreach (var model in GetModels(application))
            {
                registry.RegisterTemplate(TemplateId, context => CreateTemplateInstance(context, model));
            }
        }
    }
}