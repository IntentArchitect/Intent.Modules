using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java;

/// <summary>
/// Extension methods for <see cref="IOutputTarget"/>.
/// </summary>
[Obsolete(WillBeRemovedIn.Version4)]
public static class JavaOutputTargetExtensions
{
    /// <summary>
    /// Creates a fully qualified package structure based on the provided <paramref name="target"/>.
    /// </summary>
    [FixFor_Version4("Remove this method and make the overload use the params keyword for the last parameter.")]
    public static string GetPackage(this IOutputTarget target)
    {
        return target.GetPackage(Array.Empty<string>());
    }

    /// <summary>
    /// Creates a fully qualified package structure based on the provided <paramref name="outputTarget"/>.
    /// </summary>
    public static string GetPackage(this IOutputTarget outputTarget, string[] additionalFolders)
    {
        var packages = outputTarget.GetTargetPath()
            .Select(x => x.Name)
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
    /// Creates a folder path for the provided <paramref name="outputTarget"/> and also applies
    /// Java package identifier conventions as appropriate.
    /// </summary>
    public static string GetPackageFolderPath(this IOutputTarget outputTarget, params string[] additionalFolders)
    {
        var path = additionalFolders
            .ToArray();

        for (var index = path.Length - 1; index >= 0; index--)
        {
            if (path[index] == "java")
            {
                break;
            }

            path[index] = path[index].ToJavaPackage();
        }

        return string.Join('/', path);
    }
}