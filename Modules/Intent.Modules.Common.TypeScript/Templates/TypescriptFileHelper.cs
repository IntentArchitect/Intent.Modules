using System.IO;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Editor;
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
                if (!(dependency is IHasClassDetails))
                {
                    continue;
                }

                file.AddImportIfNotExists(
                    className: ((IHasClassDetails)dependency).ClassName,
                    location: template.GetMetadata().GetFilePath().GetRelativePath(dependency.GetMetadata().GetFilePathWithoutExtension()).Normalize());
            }
        }
    }
}