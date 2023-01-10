using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Dart.Api;
using Intent.Modules.ModuleBuilder.Dart.Templates.Templates.DartTemplatePartial;

using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Dart.Templates.Templates
{
    public static class TemplateExtensions
    {
        public static string GetTypescriptTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : DartFileTemplateModel
        {
            return template.GetTypeName(DartTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetTypescriptTemplatePartialName(this IntentTemplateBase template, DartFileTemplateModel model)
        {
            return template.GetTypeName(DartTemplatePartialTemplate.TemplateId, model);
        }

        //public static string GetTypescriptTemplateStringInterpolationName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel
        //{
        //    return template.GetTypeName(TypescriptTemplateStringInterpolationTemplate.TemplateId, template.Model);
        //}

        //public static string GetTypescriptTemplateStringInterpolationName(this IntentTemplateBase template, Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel model)
        //{
        //    return template.GetTypeName(TypescriptTemplateStringInterpolationTemplate.TemplateId, model);
        //}

    }
}

