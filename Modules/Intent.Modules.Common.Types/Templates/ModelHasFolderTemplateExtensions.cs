using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common
{
    public static class ModelHasFolderTemplateExtensions
    {
        /// <summary>
        /// Creates a folder path based on the parent folders of this template's model as described in the designer.
        /// </summary>
        public static string GetFolderPath<TModel>(this IntentTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join("/", template.Model.GetParentFolderNames().Concat(additionalFolders));
        }

        /// <summary>
        /// Creates a folder path based on the <paramref name="additionalFolders"/>.
        /// </summary>
        public static string GetFolderPath(this IntentTemplateBase<object> template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }

        /// <summary>
        /// Creates a folder path based on the <paramref name="additionalFolders"/>.
        /// </summary>
        public static string GetFolderPath(this IntentTemplateBase template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }
    }
}