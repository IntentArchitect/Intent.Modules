using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial;
using Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplateStringInterpolation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript.Templates
{
    public static class TemplateExtensions
    {
        public static string GetTypescriptTemplatePartialName<T>(this IIntentTemplate<T> template) where T : Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel
        {
            return template.GetTypeName(TypescriptTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetTypescriptTemplatePartialName(this IIntentTemplate template, Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel model)
        {
            return template.GetTypeName(TypescriptTemplatePartialTemplate.TemplateId, model);
        }

        public static string GetTypescriptTemplateStringInterpolationName<T>(this IIntentTemplate<T> template) where T : Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel
        {
            return template.GetTypeName(TypescriptTemplateStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetTypescriptTemplateStringInterpolationName(this IIntentTemplate template, Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel model)
        {
            return template.GetTypeName(TypescriptTemplateStringInterpolationTemplate.TemplateId, model);
        }

    }
}