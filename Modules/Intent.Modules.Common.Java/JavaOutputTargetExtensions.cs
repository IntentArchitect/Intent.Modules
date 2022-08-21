using System;
using Intent.Engine;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java;

/// <summary>
/// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods"/> instead.
/// </summary>
[Obsolete(WillBeRemovedIn.Version4)]
public static class JavaOutputTargetExtensions
{
    /// <summary>
    /// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods.GetPackage{TModel}"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackage(this IOutputTarget target)
    {
        return GetPackage(target, null);
    }

    /// <summary>
    /// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods.GetPackage{TModel}"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackage(this IOutputTarget outputTarget, string[] additionalFolders)
    {
        return JavaTemplateBaseExtensionMethods.GetPackageStructure(
            outputTarget: outputTarget,
            hasFolder: null,
            additionalFolders: additionalFolders);
    }

    /// <summary>
    /// Obsolete. Use <see cref="ModelHasFolderTemplateExtensions.GetFolderPath(IntentTemplateBase,string[])"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackageFolderPath(this IOutputTarget _, params string[] additionalFolders)
    {
        return string.Join('/', additionalFolders);
    }
}