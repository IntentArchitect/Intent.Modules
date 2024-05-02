using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <inheritdoc />
    public abstract class RazorTemplateBase : RazorTemplateBase<object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="RazorTemplateBase"/>.
        /// </summary>
        protected RazorTemplateBase(string templateId, IOutputTarget outputTarget, object model) : base(templateId, outputTarget, model)
        {
        }
    }

    /// <inheritdoc cref="CSharpTemplateBase{TModel}"/>
    public abstract class RazorTemplateBase<TModel> : CSharpTemplateBase<TModel>, IRazorMerge
    {
        /// <summary>
        /// Creates a new instance of <see cref="RazorTemplateBase{TModel}"/>.
        /// </summary>
        protected RazorTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        protected sealed override CSharpFileConfig DefineFileConfig() => DefineRazorConfig();

        /// <summary>
        /// Factory method for creating a <see cref="RazorFileConfig"/> for a template.
        /// </summary>
        protected abstract RazorFileConfig DefineRazorConfig();

        /// <inheritdoc />
        protected override IEnumerable<string> GetUsingsFromContent(string existingContent) => [];
    }
}
