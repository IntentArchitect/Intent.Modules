using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Dart.Templates;

/// <summary>
/// Helpers for Dart files.
/// </summary>
public static class DartFileHelper
{
    /// <summary>
    /// Returns the relative file path between this template and another.
    /// </summary>
    public static string GetRelativePath<T>(this DartTemplateBase<T> template, ITemplate dependency)
    {
        return "./" + template.GetMetadata().GetFullLocationPath().GetRelativePath(dependency.GetMetadata().GetFilePathWithoutExtension());
    }
}