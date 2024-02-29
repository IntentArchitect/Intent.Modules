using System.Collections.Generic;
using Intent.ModuleBuilder.Java.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Java.Templates.JavaFileStringInterpolation;
using Intent.Modules.ModuleBuilder.Java.Templates.JavaFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Java.Templates
{
    public static class TemplateExtensions
    {
        public static string GetJavaFileStringInterpolationTemplateName<T>(this IIntentTemplate<T> template) where T : JavaFileTemplateModel
        {
            return template.GetTypeName(JavaFileStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFileStringInterpolationTemplateName(this IIntentTemplate template, JavaFileTemplateModel model)
        {
            return template.GetTypeName(JavaFileStringInterpolationTemplate.TemplateId, model);
        }

        public static string GetJavaFileTemplatePartialName<T>(this IIntentTemplate<T> template) where T : JavaFileTemplateModel
        {
            return template.GetTypeName(JavaFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFileTemplatePartialName(this IIntentTemplate template, JavaFileTemplateModel model)
        {
            return template.GetTypeName(JavaFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}