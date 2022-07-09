using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpStringInterpolation;
using Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCSharpStringInterpolationTemplateName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.CSharp.Api.CSharpTemplateModel
        {
            return template.GetTypeName(CSharpStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpStringInterpolationTemplateName(this IntentTemplateBase template, Intent.ModuleBuilder.CSharp.Api.CSharpTemplateModel model)
        {
            return template.GetTypeName(CSharpStringInterpolationTemplate.TemplateId, model);
        }
        public static string GetCSharpTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.CSharp.Api.CSharpTemplateModel
        {
            return template.GetTypeName(CSharpTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpTemplatePartialName(this IntentTemplateBase template, Intent.ModuleBuilder.CSharp.Api.CSharpTemplateModel model)
        {
            return template.GetTypeName(CSharpTemplatePartialTemplate.TemplateId, model);
        }

    }
}