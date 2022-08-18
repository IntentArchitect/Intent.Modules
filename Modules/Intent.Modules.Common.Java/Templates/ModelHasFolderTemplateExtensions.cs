using System;
using Intent.Modules.Common.Types.Api;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java.Templates;

/// <summary>
/// Use <see cref="JavaTemplateBaseExtensionMethods"/> instead.
/// </summary>
[Obsolete(WillBeRemovedIn.Version4)]
public static class ModelHasFolderTemplateExtensions
{
    /// <summary>
    /// Use <see cref="JavaTemplateBaseExtensionMethods.GetPackage{TModel}"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackage<TModel>(JavaTemplateBase<TModel> template, params string[] additionalFolders)
        where TModel : IHasFolder
    {
        return template.GetPackage(additionalFolders);
    }

    /// <summary>
    /// Use <see cref="JavaTemplateBaseExtensionMethods.GetPackageFolderPath{TModel}"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackageFolderPath<TModel>(JavaTemplateBase<TModel> template, params string[] additionalFolders)
        where TModel : IHasFolder
    {
        return template.GetPackageFolderPath(additionalFolders);
    }
}