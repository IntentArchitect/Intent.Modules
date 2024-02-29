using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.TypeScript.Weaving.Decorators.Templates.IntentDecorators;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.TypeScript.Weaving.Decorators.Templates
{
    public static class TemplateExtensions
    {
        public static string GetIntentDecoratorsName(this IIntentTemplate template)
        {
            return template.GetTypeName(IntentDecoratorsTemplate.TemplateId);
        }

    }
}