using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Kotlin.Templates
{
    public static class KotlinFileHelper
    {
        public static string[] ResolveAllImports<T>(this KotlinTemplateBase<T> template, params string[] importsToIgnore)
        {
            var imports = template
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

            foreach (var @using in imports.Where(x => x != FixImport(x)))
            {
                Logging.Log.Warning($"When resolving imports for Template Id [{template.Id}] file [{template.GetMetadata().FileName}], " +
                                    $"an import arrived with with the format [{@using}], but should have been in the format " +
                                    $"[{FixImport(@using)}]. The module and/or decorator author should update this module.");
            }

            imports = imports
                .Select(x => $"import {FixImport(x)}")
                .Distinct()
                .ToArray();

            return imports;
        }

        private static string FixImport(string @using)
        {
            return @using.RemovePrefix("import ").RemoveSuffix(";");
        }

        /// <summary>
        /// This member will be changed to be only privately accessible or possibly removed
        /// entirely, please contact Intent Architect support should you have a dependency on it.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        [FixFor_Version4("See comments in method.")]
        public static IEnumerable<string> GetAllDeclareUsing(this ITemplate template)
        {
            // No apparent reason this can't be private or possibly even a local method.
            return template.GetAll<IDeclareImports, string>((i) => i.DeclareImports());
        }
    }
}