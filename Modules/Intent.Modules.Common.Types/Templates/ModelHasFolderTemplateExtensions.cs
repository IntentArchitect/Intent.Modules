using System;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common
{
    public static class ModelHasFolderTemplateExtensionsV2
    {
        /// <summary>
        /// Creates a folder path based on the parent folders of this template's model as described in the designer.
        /// </summary>
        public static string GetFolderPath<TModel>(this IIntentTemplate<TModel> template, params string[] additionalFolders)
        {
            if (template.Model is not IHasFolder hasFolder)
            {
                return string.Join("/", additionalFolders);
            }

            return string.Join("/", hasFolder.GetParentFolderNames().Concat(additionalFolders));
        }

        /// <summary>
        /// Creates a folder path based on the <paramref name="additionalFolders"/>.
        /// </summary>
        public static string GetFolderPath(this IIntentTemplate template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }
    }

    /// <summary>
    /// Obsolete. Use <see cref="ModelHasFolderTemplateExtensionsV2"/> instead.
    /// </summary>
    [Obsolete]
    public static class ModelHasFolderTemplateExtensions
    {
        /// <summary>
        /// Obsolete. <see cref="ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}"/> should be used instead.
        /// This is maintained with being a non-extension method to maintain backwards compatibility for code already
        /// compiled against this method.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath<TModel>(IntentTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join("/", template.Model.GetParentFolderNames().Concat(additionalFolders));
        }

        /// <summary>
        /// Obsolete. <see cref="ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}"/> should be used instead.
        /// This is maintained with being a non-extension method to maintain backwards compatibility for code already
        /// compiled against this method.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath<TModel>(IIntentTemplate<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder<IFolder>
        {
            return string.Join("/", template.Model.GetParentFolderNames().Concat(additionalFolders));
        }

        /// <summary>
        /// Obsolete. <see cref="ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}"/> should be used instead.
        /// This is maintained with being a non-extension method to maintain backwards compatibility for code already
        /// compiled against this method.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath(IntentTemplateBase<object> template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }

        /// <summary>
        /// Obsolete. <see cref="ModelHasFolderTemplateExtensionsV2.GetFolderPath"/> should be used instead.
        /// This is maintained with being a non-extension method to maintain backwards compatibility for code already
        /// compiled against this method.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath(IntentTemplateBase template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }
    }
}