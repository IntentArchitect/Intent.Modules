using System.Linq;
using Intent.Code.Weaving.Java.Editor;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    public static class JavaFileHelper
    {
        public static void AddDependencyImports<T>(this JavaFile file, JavaTemplateBase<T> template)
        {
            var dependencies = template.GetTemplateDependencies().Select(template.ExecutionContext.FindTemplateInstance<ITemplate>).Distinct();
            foreach (var dependency in dependencies)
            {
                var classProvider = dependency as IClassProvider;
                if (classProvider == null)
                {
                    continue;
                }

                if (file.ImportExists($@"import {classProvider.Namespace}.{classProvider.ClassName};"))
                {
                    continue;
                }

                file.AddImport($@"
import {classProvider.Namespace}.{classProvider.ClassName};");
            }
        }
    }
}