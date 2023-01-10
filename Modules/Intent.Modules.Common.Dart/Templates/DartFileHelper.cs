using System.Linq;
//using Intent.Code.Weaving.Dart.Editor;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Dart.Templates
{
    public static class DartFileHelper
    {
        //public static void AddDependencyImports<T>(this TDartFileHelperFile file, TDartFileHelperTemplateBase<T> template)
        //{
        //    var dependencies = template.GetTemplateDependencies().Select(template.ExecutionContext.FindTemplateInstance<ITemplate>).Distinct();
        //    foreach (var dependency in dependencies)
        //    {
        //        if (dependency is not IClassProvider classProvider)
        //        {
        //            continue;
        //        }

        //        if (string.IsNullOrWhiteSpace(classProvider.ClassName))
        //        {
        //            continue;
        //        }

        //        file.AddImportIfNotExists(
        //            className: classProvider.ClassName,
        //            location: GetRelativePath(template, classProvider));
        //    }

        //    foreach (var import in template.Imports)
        //    {
        //        file.AddImportIfNotExists(
        //            className: import.Type,
        //            location: import.Location);
        //    }
        //    file.NormalizeImports();
        //}

        public static string GetRelativePath<T>(this DartTemplateBase<T> template, ITemplate dependency)
        {
            return "./" + template.GetMetadata().GetFullLocationPath().GetRelativePath(dependency.GetMetadata().GetFilePathWithoutExtension());
        }
    }
}