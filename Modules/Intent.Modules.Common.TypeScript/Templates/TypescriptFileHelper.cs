using System.IO;
using System.Linq;
using Intent.Code.Weaving.TypeScript.Editor;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeScript.Templates
{
    public static class TypescriptFileHelper
    {
        public static void AddDependencyImports<T>(this TypeScriptFile file, TypeScriptTemplateBase<T> template)
        {
            var dependencies = template.GetTemplateDependencies().Select(template.ExecutionContext.FindTemplateInstance<ITemplate>).Distinct();
            foreach (var dependency in dependencies)
            {
                if (!(dependency is IClassProvider))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(((IClassProvider) dependency).ClassName))
                {
                    continue;
                }

                file.AddImportIfNotExists(
                    className: ((IClassProvider)dependency).ClassName,
                    location: GetRelativePath(template, dependency));
            }

            foreach (var import in template.Imports)
            {
                file.AddImportIfNotExists(
                    className: import.Type,
                    location: import.Location);
            }
            file.NormalizeImports();
        }

        public static string GetRelativePath<T>(this TypeScriptTemplateBase<T> template, ITemplate dependency)
        {
            return "./" + template.GetMetadata().GetFullLocationPath().GetRelativePath(dependency.GetMetadata().GetFilePathWithoutExtension());
        }
    }
}