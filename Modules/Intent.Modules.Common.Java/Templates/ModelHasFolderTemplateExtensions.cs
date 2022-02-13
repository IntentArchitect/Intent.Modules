using System;
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
        /// Creates a fully qualified package based on the OutputTarget location and the parent folders of this template's model as described in the designer.
        /// </summary>
        public static string GetPackage<TModel>(this JavaTemplateBase<TModel> template, params string[] additionalParts)
            where TModel : IHasFolder
        {
            var rawPackage = string.Join(".", template.OutputTarget.GetTargetPath().Select(x => x.Name)
                .Concat(template.Model.GetParentFolders()
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => x.Name))
                .Concat(additionalParts));

            return rawPackage.Split("java.", StringSplitOptions.RemoveEmptyEntries).Last().ToJavaPackage();
        }
    }
}
