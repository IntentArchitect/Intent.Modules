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

                file.AddImportIfNotExists(
                    className: ((IClassProvider)dependency).ClassName,
                    location: "./" + template.GetMetadata().GetFullLocationPath().GetRelativePath(dependency.GetMetadata().GetFilePathWithoutExtension()));
            }
        }
    }
}