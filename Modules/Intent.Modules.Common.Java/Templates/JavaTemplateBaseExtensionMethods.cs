using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.SdkEvolutionHelpers;

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
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string GetPackageFolderPath<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
        {
            return template.GetFolderPath(additionalFolders);
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

            var indexOfJavaPart = -1;
            var packageStructure = string.Join('.', Enumerable.Empty<string>()
                .Concat(outputTargetParts ?? Enumerable.Empty<string>())
                .Concat(hasFolderParts ?? Enumerable.Empty<string>())
                .Concat(additionalFolders ?? Enumerable.Empty<string>())
                .Select((part, index) =>
                {
                    if (indexOfJavaPart == -1 &&
                        part == "java")
                    {
                        indexOfJavaPart = index;
                    }

                    return part.ToJavaIdentifier(CapitalizationBehaviour.AsIs);
                })
                .ToArray() // Unless materialized here, .SkipWhile is called before .Select
                .SkipWhile((_, index) => index <= indexOfJavaPart));

            return packageStructure;
        }
    }
}