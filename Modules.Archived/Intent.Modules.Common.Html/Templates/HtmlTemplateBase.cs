using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Html.Templates
{
    public abstract class HtmlTemplateBase : HtmlTemplateBase<object>
    {
        protected HtmlTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    public abstract class HtmlTemplateBase<TModel> : IntentTemplateBase<TModel>, IHtmlFileMerge
    {
        protected HtmlTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }
    }
}