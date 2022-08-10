using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Java.Templates.JavaFileStringInterpolationTemplate;
using Intent.Modules.ModuleBuilder.Java.Templates.JavaFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Java.Templates
{
    public static class TemplateExtensions
    {
        public static string GetJavaFileStringInterpolationTemplateName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.Java.Api.JavaFileTemplateModel
        {
            return template.GetTypeName(JavaFileStringInterpolationTemplate.JavaFileStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFileStringInterpolationTemplateName(this IntentTemplateBase template, Intent.ModuleBuilder.Java.Api.JavaFileTemplateModel model)
        {
            return template.GetTypeName(JavaFileStringInterpolationTemplate.JavaFileStringInterpolationTemplate.TemplateId, model);
        }
        public static string GetJavaFileTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.Java.Api.JavaFileTemplateModel
        {
            return template.GetTypeName(JavaFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFileTemplatePartialName(this IntentTemplateBase template, Intent.ModuleBuilder.Java.Api.JavaFileTemplateModel model)
        {
            return template.GetTypeName(JavaFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}