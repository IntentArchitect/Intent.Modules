using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Code.Weaving.Java.Editor;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.Java.Templates
{
    /// <summary>
    /// Extension methods for <see cref="JavaTemplateBase{TModel}"/>.
    /// </summary>
    public static class JavaTemplateBaseExtensionMethods
    {
        /// <summary>
        /// Creates a fully qualified package structure based on the <see cref="IOutputTarget"/>
        /// location and if <typeparamref name="TModel"/> is a <see cref="IHasFolder"/> then the
        /// parent folders of the <paramref name="template"/>'s model as described in the designer
        /// will be used as well. The package structure will be created until the root or a folder
        /// named <c>java</c> is found.
        /// </summary>
        public static string GetPackage<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
        {
            return GetPackageStructure(
                outputTarget: template.OutputTarget,
                hasFolder: template.Model as IHasFolder,
                additionalFolders: additionalFolders);
        }

        /// <summary>
        /// Obsolete. Use <see cref="Common.ModelHasFolderTemplateExtensions.GetFolderPath{TModel}(IntentTemplateBase{TModel},string[])"/> instead.
        /// </summary>
        public static string GetPackageFolderPath<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
        {
            return template.GetFolderPath(additionalFolders);
        }

        /// <summary>
        /// Determines all imports for the provided <paramref name="template"/> and adds them to
        /// the provided <paramref name="file"/>.
        /// </summary>
        public static void ResolveAndAddImports<TModel>(this JavaTemplateBase<TModel> template, JavaFile file)
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

        /// <summary>
        /// Unit testable generic implementation to get a package structure.
        /// </summary>
        internal static string GetPackageStructure(
            IOutputTarget outputTarget,
            IHasFolder hasFolder,
            IEnumerable<string> additionalFolders)
        {
            var outputTargetParts = outputTarget?.GetTargetPath()
                .SelectMany(x => x.Name.Split('.'));
            var hasFolderParts = hasFolder?.GetParentFolders()
                .SelectMany(x => x.Name.Split('.'));
            additionalFolders = additionalFolders?
                .SelectMany(x => x.Split('.'));

            var packageStructure = string.Join('.', Enumerable.Empty<string>()
                .Concat(outputTargetParts ?? Enumerable.Empty<string>())
                .Concat(hasFolderParts ?? Enumerable.Empty<string>())
                .Concat(additionalFolders ?? Enumerable.Empty<string>())
                .Select(x => x.ToJavaIdentifier(CapitalizationBehaviour.AsIs)));
            
            var lastIndexOf = packageStructure.LastIndexOf("java.", StringComparison.Ordinal);
            return lastIndexOf >= 0
                ? packageStructure[(lastIndexOf + "java.".Length)..]
                : packageStructure;
        }
    }
}