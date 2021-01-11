using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Code.Weaving.Java.Editor;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Java.Templates
{
    public static class JavaFileHelper
    {
        public static void ResolveAndAddImports<T>(this JavaTemplateBase<T> template, JavaFile file)
        {
            var dependencies = template.GetTemplateDependencies().Select(template.ExecutionContext.FindTemplateInstance<ITemplate>).Distinct();
            foreach (var import in template.ResolveAllImports())
            {

                if (file.ImportExists(import) || template.Package == FixImport(import).RemoveSuffix("." + FixImport(import).Split('.').Last()))
                {
                    continue;
                }

                file.AddImport($@"
{import}");
            }
        }

        public static string[] ResolveAllImports<T>(this JavaTemplateBase<T> template, params string[] importsToIgnore)
        {
            var usings = template
                .GetAllTemplateDependencies()
                .SelectMany(template.ExecutionContext.FindTemplateInstances<ITemplate>)
                .Where(ti => ti is IClassProvider)
                .Cast<IClassProvider>()
                .ToList()
                .Select(x => $"{x.Namespace}.{x.ClassName}")
                .Union(template.GetAllDeclareUsing())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Except(importsToIgnore)
                .Distinct()
                .ToArray();

            foreach (var @using in usings.Where(x => x != FixImport(x)))
            {
                Logging.Log.Warning($"When resolving imports for Template Id [{template.Id}] file [{template.GetMetadata().FileName}], " +
                                    $"an import arrived with with the format [{@using}], but should have been in the format " +
                                    $"[{FixImport(@using)}]. The module and/or decorator author should update this module.");
            }

            usings = usings
                .Select(x => $"import {FixImport(x)};")
                .Distinct()
                .ToArray();

            return usings;
            //return usings.Any()
            //    ? usings.Aggregate((x, y) => x + Environment.NewLine + y)
            //    : string.Empty;
        }

        private static string FixImport(string @using)
        {
            return @using.RemovePrefix("import ").RemoveSuffix(";");
        }

        public static IEnumerable<string> GetAllDeclareUsing(this ITemplate template)
        {
            return template.GetAll<IDeclareImports, string>((i) => i.DeclareImports());
        }
    }

    public interface IDeclareImports
    {
        IEnumerable<string> DeclareImports();
    }
}