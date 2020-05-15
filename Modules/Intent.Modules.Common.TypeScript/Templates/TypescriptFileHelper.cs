using System;
using System.Linq;
using System.Text;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Angular.Templates
{
    public static class TypescriptFileHelper
    {
        public static void AddDependencyImports<T>(this TypescriptFile file, AngularTypescriptProjectItemTemplateBase<T> template)
        {
            var dependencies = template.GetTemplateDependencies().Select(template.Project.FindTemplateInstance<ITemplate>).Distinct();
            foreach (var dependency in dependencies)
            {
                if (!(dependency is IHasClassDetails))
                {
                    continue;
                }

                file.AddImportIfNotExists(
                    className: ((IHasClassDetails)dependency).ClassName,
                    location: template.GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(dependency.GetMetadata().GetRelativeFilePathWithFileName()));
            }
        }
    }
}