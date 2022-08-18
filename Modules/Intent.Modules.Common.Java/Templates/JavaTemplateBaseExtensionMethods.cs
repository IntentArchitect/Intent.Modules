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
    /// Extension methods for <see cref="JavaTemplateBase"/> and
    /// <see cref="JavaTemplateBase{TModel}"/>.
    /// </summary>
    public static class JavaTemplateBaseExtensionMethods
    {
        /// <summary>
        /// Creates a fully qualified package structure based on the <see cref="IOutputTarget"/>
        /// location and if <typeparamref name="TModel"/> is a <see cref="IHasFolder"/> then the
        /// parent folders of the <paramref name="template"/>'s model as described in the designer
        /// will be used as well.
        /// </summary>
        /// <remarks>
        /// Both the folder path of the provided <typeparamref name="TModel"/>'s location within
        /// its designer and <paramref name="additionalFolders"/> have package naming conventions
        /// applied to them.
        /// <para>
        /// The package names dictated by a template's <see cref="IOutputTarget"/> do not have package
        /// naming conventions applied to them because templates cannot control this part of the
        /// output path. These package names can be changed by updating folder names in the
        /// <c>Folder Structure</c> designer.
        /// </para>
        /// </remarks>
        public static string GetPackage<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
        {
            var result = GetRelativeFolderAndPackageStructure(
                outputTarget: template.OutputTarget,
                hasFolder: template.Model as IHasFolder,
                additionalFolders: additionalFolders);

            return result.PackageStructure;
        }

        /// <summary>
        /// Creates a folder path for the provided <paramref name="additionalFolders"/> and applies
        /// Java package naming conventions as appropriate with Java package naming conventions
        /// applied. If <typeparamref name="TModel"/> is a <see cref="IHasFolder"/> then its path
        /// will also be included as described in the designer.
        /// </summary>
        public static string GetPackageFolderPath<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
        {
            var result = GetRelativeFolderAndPackageStructure(
                outputTarget: null,
                hasFolder: template.Model as IHasFolder, 
                additionalFolders: additionalFolders);

            return result.RelativeFolder;
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
        /// Function for use by all others which addresses all known concerns and is also covered
        /// by unit tests.
        /// </summary>
        internal static (string RelativeFolder, string PackageStructure) GetRelativeFolderAndPackageStructure(
            IOutputTarget outputTarget,
            IHasFolder hasFolder,
            IEnumerable<string> additionalFolders)
        {
            var outputTargetParts = outputTarget?.GetTargetPath()
                .Select(x => x.Name);
            var hasFolderParts = hasFolder?.GetParentFolders()
                .Select(x => x.Name);

            var packages = Enumerable.Empty<PackagePart>()
                .Concat((outputTargetParts ?? Enumerable.Empty<string>())
                    .Select(x => new PackagePart(x, true)))
                .Concat((hasFolderParts ?? Enumerable.Empty<string>())
                    .Select(x => new PackagePart(x, false)))
                .Concat((additionalFolders ?? Enumerable.Empty<string>())
                    .Select(x => new PackagePart(x, false)))
                .ToArray();

            var foundSourceRoot = false;
            for (var packageIndex = packages.Length - 1; packageIndex >= 0; packageIndex--)
            {
                var package = packages[packageIndex];
                for (var partIndex = package.FolderParts.Length - 1; partIndex >= 0; partIndex--)
                {
                    if (package.FolderParts[partIndex] == "java")
                    {
                        foundSourceRoot = true;
                    }

                    if (foundSourceRoot)
                    {
                        break;
                    }

                    if (!package.IsOutputTargetPart)
                    {
                        package.FolderParts[partIndex] = package.FolderParts[partIndex].ToJavaPackage();
                    }

                    package.PackageParts[partIndex] = package.FolderParts[partIndex];
                }

                if (foundSourceRoot)
                {
                    break;
                }
            }

            var relativeFolder = string.Join('/', packages
                .Where(x => !x.IsOutputTargetPart)
                .Select(x => string.Join('.', x.FolderParts)));

            var packageStructure = string.Join('.', packages
                .SelectMany(package => package.PackageParts)
                .Where(x => !string.IsNullOrWhiteSpace(x)));

            return (relativeFolder, packageStructure);
        }

        private class PackagePart
        {
            public PackagePart(string parts, bool isOutputTargetPart)
            {
                FolderParts = parts.Split('.');
                PackageParts = new string[FolderParts.Length];
                IsOutputTargetPart = isOutputTargetPart;
            }

            public string[] PackageParts { get; }

            public string[] FolderParts { get; }

            /// <remarks>
            /// Templates don't control the folder names of output target parts of the path, so
            /// to avoid a misalignment of the package name and folder path, we leave them as
            /// is. Users can fix folder names as desired in the `Folder Structure` designer.
            /// </remarks>
            public bool IsOutputTargetPart { get; }
        }
    }
}