using System;
using Intent.Engine;
using Intent.Modules.Common.Java.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java;

/// <summary>
/// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods"/> instead.
/// </summary>
[Obsolete(WillBeRemovedIn.Version4)]
public static class JavaOutputTargetExtensions
{
    /// <summary>
    /// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods.GetPackage"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackage(this IOutputTarget target)
    {
        var result = JavaTemplateBaseExtensionMethods.GetRelativeFolderAndPackageStructure(
            outputTarget: target,
            hasFolder: null,
            additionalFolders: null);

        return result.PackageStructure;
    }

    /// <summary>
    /// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods.GetPackage"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackage(this IOutputTarget outputTarget, string[] additionalFolders)
    {
        var result = JavaTemplateBaseExtensionMethods.GetRelativeFolderAndPackageStructure(
            outputTarget: outputTarget,
            hasFolder: null,
            additionalFolders: additionalFolders);

        return result.PackageStructure;
    }

    /// <summary>
    /// Obsolete. Use <see cref="JavaTemplateBaseExtensionMethods.GetPackageFolderPath"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static string GetPackageFolderPath(this IOutputTarget outputTarget, params string[] additionalFolders)
    {
        var result = JavaTemplateBaseExtensionMethods.GetRelativeFolderAndPackageStructure(
            outputTarget: outputTarget,
            hasFolder: null,
            additionalFolders: additionalFolders);

        return result.RelativeFolder;
    }
}