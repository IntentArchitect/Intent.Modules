using Intent.Modules.Common.TypeScript.Templates;
using Intent.Modules.TypeScript.Weaving.Decorators.Templates.IntentDecorators;

namespace Intent.Modules.TypeScript.Weaving.Decorators
{
    public static class IntentDecorators
    {
        public static string IntentIgnoreDecorator<T>(this TypeScriptTemplateBase<T> template)
        {
            var intentDecorators = template.GetTemplate<IntentDecoratorsTemplate>(IntentDecoratorsTemplate.TemplateId);
            template.AddImport(intentDecorators.IntentIgnore, template.GetRelativePath(intentDecorators));
            return $"@{intentDecorators.IntentIgnore}()";
        }

        public static string IntentIgnoreBodyDecorator<T>(this TypeScriptTemplateBase<T> template)
        {
            var intentDecorators = template.GetTemplate<IntentDecoratorsTemplate>(IntentDecoratorsTemplate.TemplateId);
            template.AddImport(intentDecorators.IntentIgnoreBody, template.GetRelativePath(intentDecorators));
            return $"@{intentDecorators.IntentIgnoreBody}()";
        }

        public static string IntentMergeDecorator<T>(this TypeScriptTemplateBase<T> template)
        {
            var intentDecorators = template.GetTemplate<IntentDecoratorsTemplate>(IntentDecoratorsTemplate.TemplateId);
            template.AddImport(intentDecorators.IntentMerge, template.GetRelativePath(intentDecorators));
            return $"@{intentDecorators.IntentMerge}()";
        }

        public static string IntentManageDecorator<T>(this TypeScriptTemplateBase<T> template)
        {
            var intentDecorators = template.GetTemplate<IntentDecoratorsTemplate>(IntentDecoratorsTemplate.TemplateId);
            template.AddImport(intentDecorators.IntentManage, template.GetRelativePath(intentDecorators));
            return $"@{intentDecorators.IntentManage}()";
        }

        public static string IntentManageClassDecorator<T>(this TypeScriptTemplateBase<T> template)
        {
            var intentDecorators = template.GetTemplate<IntentDecoratorsTemplate>(IntentDecoratorsTemplate.TemplateId);
            template.AddImport(intentDecorators.IntentManageClass, template.GetRelativePath(intentDecorators));
            return $"@{intentDecorators.IntentManageClass}()";
        }
    }
}