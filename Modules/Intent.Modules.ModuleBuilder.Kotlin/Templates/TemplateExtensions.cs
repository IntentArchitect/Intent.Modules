using System.Collections.Generic;
using Intent.ModuleBuilder.Kotlin.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Kotlin.Templates
{
    public static class TemplateExtensions
    {
        public static string GetKotlinFileTemplatePartialName<T>(this IIntentTemplate<T> template) where T : KotlinFileTemplateModel
        {
            return template.GetTypeName(KotlinFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetKotlinFileTemplatePartialName(this IIntentTemplate template, KotlinFileTemplateModel model)
        {
            return template.GetTypeName(KotlinFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}