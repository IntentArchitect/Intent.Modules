using System;
using System.Linq;
using Intent.Code.Weaving.Java.Editor;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    /// <summary>
    /// Extension methods for <see cref="JavaTemplateBase{TModel}"/>.
    /// </summary>
    public static class JavaTemplateBaseExtensionMethods
    {
        /// <summary>
        /// Determines all imports for the provided <paramref name="template"/> and adds them to
        /// the provided <paramref name="file"/>.
        /// </summary>
        public static void ResolveAndAddImports<T>(this JavaTemplateBase<T> template, JavaFile file)
        {
            var imports = template.GetAll<IDeclareImports, string>(item => item.DeclareImports())
                .Where(import => !string.IsNullOrWhiteSpace(import))
                .Distinct();

            foreach (var import in imports)
            {
                if (file.ImportExists(import) || template.Package == import.RemoveSuffix("." + import.Split('.').Last()))
                {
                    continue;
                }

                file.AddImport($"{Environment.NewLine}import {import};");
            }
        }
    }
}