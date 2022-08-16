using System.Linq;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.Java.Templates
{
    /// <summary>
    /// Java extension methods.
    /// </summary>
    public static class ModelHasFolderTemplateExtensions
    {
        /// <summary>
        /// Creates a fully qualified package structure based on the OutputTarget location and the
        /// parent folders of the <paramref name="template"/>'s model as described in the designer.
        /// </summary>
        public static string GetPackage<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            var packages = template.OutputTarget.GetTargetPath()
                .Select(x => x.Name)
                .Concat(template.Model.GetParentFolders()
                    .Where(x => x.GetStereotype("Folder Options")?.GetProperty<bool>("Package Provider") is null or true)
                    .Select(x => x.Name))
                .Concat(additionalFolders)
                .ToArray();

            var foundSourceRoot = false;
            for (var index = packages.Length - 1; index >= 0; index--)
            {
                if (packages[index] == "java")
                {
                    foundSourceRoot = true;
                }

                packages[index] = foundSourceRoot
                    ? null
                    : packages[index].ToJavaPackage();
            }

            return string.Join(".", packages.Where(package => !string.IsNullOrWhiteSpace(package)));
        }

        /// <summary>
        /// Creates a folder path for the template based on its model as described in the designer and
        /// also applies Java package identifier conventions.
        /// </summary>
        public static string GetPackageFolderPath<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            var path = template.Model.GetParentFolders()
                .Where(x => x.GetStereotype("Folder Options")?.GetProperty<bool>("Package Provider") is null or true)
                .Select(x => x.Name)
                .Concat(additionalFolders)
                .ToArray();

            for (var index = path.Length - 1; index >= 0; index--)
            {
                if (path[index] == "java")
                {
                    break;
                }

                path[index] = path[index].ToJavaPackage();
            }

            return string.Join("/", path);
        }
    }
}
