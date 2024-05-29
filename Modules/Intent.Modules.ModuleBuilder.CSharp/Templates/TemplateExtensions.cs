using System.Collections.Generic;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpStringInterpolation;
using Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial;
using Intent.Modules.ModuleBuilder.CSharp.Templates.RazorStringInterpolation;
using Intent.Modules.ModuleBuilder.CSharp.Templates.RazorTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCSharpStringInterpolationTemplateName<T>(this IIntentTemplate<T> template) where T : CSharpTemplateModel
        {
            return template.GetTypeName(CSharpStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpStringInterpolationTemplateName(this IIntentTemplate template, CSharpTemplateModel model)
        {
            return template.GetTypeName(CSharpStringInterpolationTemplate.TemplateId, model);
        }

        public static string GetCSharpTemplatePartialName<T>(this IIntentTemplate<T> template) where T : CSharpTemplateModel
        {
            return template.GetTypeName(CSharpTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpTemplatePartialName(this IIntentTemplate template, CSharpTemplateModel model)
        {
            return template.GetTypeName(CSharpTemplatePartialTemplate.TemplateId, model);
        }

        public static string GetRazorStringInterpolationTemplateName<T>(this IIntentTemplate<T> template) where T : RazorTemplateModel
        {
            return template.GetTypeName(RazorStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetRazorStringInterpolationTemplateName(this IIntentTemplate template, RazorTemplateModel model)
        {
            return template.GetTypeName(RazorStringInterpolationTemplate.TemplateId, model);
        }

        public static string GetRazorTemplatePartialName<T>(this IIntentTemplate<T> template) where T : RazorTemplateModel
        {
            return template.GetTypeName(RazorTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetRazorTemplatePartialName(this IIntentTemplate template, RazorTemplateModel model)
        {
            return template.GetTypeName(RazorTemplatePartialTemplate.TemplateId, model);
        }

    }
}