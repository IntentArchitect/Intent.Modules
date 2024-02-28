using System.Collections.Generic;
using Intent.ModuleBuilder.Html.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Html.Templates.HtmlFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Html.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHtmlFileTemplatePartialName<T>(this IIntentTemplate<T> template) where T : HtmlFileTemplateModel
        {
            return template.GetTypeName(HtmlFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetHtmlFileTemplatePartialName(this IIntentTemplate template, HtmlFileTemplateModel model)
        {
            return template.GetTypeName(HtmlFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}