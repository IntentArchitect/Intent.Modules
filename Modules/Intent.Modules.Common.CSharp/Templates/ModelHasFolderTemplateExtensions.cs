using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.CSharp.Templates
{
    public static class ModelHasFolderTemplateExtensions
    {
        public static string GetFolderPath<TModel>(this IntentTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join("/", template.Model.GetParentFolderNames().Concat(additionalFolders));
        }

        public static string GetNamespace<TModel>(this CSharpTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join(".", new [] { template.OutputTarget.GetNamespace() }.Concat(template.Model.GetParentFolderNames()).Concat(additionalFolders));
        }
    }
}