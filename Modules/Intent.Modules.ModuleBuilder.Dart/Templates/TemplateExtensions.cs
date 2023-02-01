using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Dart.Templates.DartFileStringInterpolation;
using Intent.Modules.ModuleBuilder.Dart.Templates.DartFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Dart.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDartFileStringInterpolationTemplateName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.Dart.Api.DartFileTemplateModel
        {
            return template.GetTypeName(DartFileStringInterpolationTemplate.TemplateId, template.Model);
        }

        public static string GetDartFileStringInterpolationTemplateName(this IntentTemplateBase template, Intent.ModuleBuilder.Dart.Api.DartFileTemplateModel model)
        {
            return template.GetTypeName(DartFileStringInterpolationTemplate.TemplateId, model);
        }

        public static string GetDartFileTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.Dart.Api.DartFileTemplateModel
        {
            return template.GetTypeName(DartFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetDartFileTemplatePartialName(this IntentTemplateBase template, Intent.ModuleBuilder.Dart.Api.DartFileTemplateModel model)
        {
            return template.GetTypeName(DartFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}