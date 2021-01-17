using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.CSharp.Templates
{
    public static class ModelHasFolderTemplateExtensions
    {
        /// <summary>
        /// Creates a folder path based on the parent folders of this template's model as described in the designer.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="additionalFolders"></param>
        /// <returns></returns>
        public static string GetFolderPath<TModel>(this IntentTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join("/", template.Model.GetParentFolderNames().Concat(additionalFolders));
        }

        /// <summary>
        /// Creates a folder path based on the <see cref="additionalFolders"/>.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="additionalFolders"></param>
        /// <returns></returns>
        public static string GetFolderPath(this IntentTemplateBase<object> template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }

        /// <summary>
        /// Creates a fully qualified namespace based on the OutputTarget location and the parent folders of this template's model as described in the designer.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="additionalFolders"></param>
        /// <returns></returns>
        public static string GetNamespace<TModel>(this CSharpTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join(".", new [] { template.OutputTarget.GetNamespace() }.Concat(template.Model.GetParentFolderNames()).Concat(additionalFolders));
        }

        /// <summary>
        /// Creates a fully qualified namespace based on the OutputTarget location and the <see cref="additionalFolders"/>.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="additionalFolders"></param>
        /// <returns></returns>
        public static string GetNamespace(this CSharpTemplateBase<object> template, params string[] additionalFolders)
        {
            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }.Concat(additionalFolders));
        }
    }
}