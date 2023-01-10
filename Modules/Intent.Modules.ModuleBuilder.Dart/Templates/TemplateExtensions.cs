using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Dart.Templates.Templates.DartTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Dart.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDartTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : Intent.Modules.ModuleBuilder.Dart.Api.DartFileTemplateModel
        {
            return template.GetTypeName(DartTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetDartTemplatePartialName(this IntentTemplateBase template, Intent.Modules.ModuleBuilder.Dart.Api.DartFileTemplateModel model)
        {
            return template.GetTypeName(DartTemplatePartialTemplate.TemplateId, model);
        }

    }
}