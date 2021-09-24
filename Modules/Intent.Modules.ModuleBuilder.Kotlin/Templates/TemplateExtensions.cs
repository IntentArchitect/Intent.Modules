using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Kotlin.Templates.Templates.KotlinFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Kotlin.Templates
{
    public static class TemplateExtensions
    {
        public static string GetKotlinFileTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.Kotlin.Api.KotlinFileTemplateModel
        {
            return template.GetTypeName(KotlinFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetKotlinFileTemplatePartialName(this IntentTemplateBase template, Intent.ModuleBuilder.Kotlin.Api.KotlinFileTemplateModel model)
        {
            return template.GetTypeName(KotlinFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}