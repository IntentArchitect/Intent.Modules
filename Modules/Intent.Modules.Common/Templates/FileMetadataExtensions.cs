using System.IO;
using Intent.Templates;

namespace Intent.Modules.Common.Templates;

/// <summary>
/// Extension methods for <see cref="IFileMetadata"/>.
/// </summary>
public static class FileMetadataExtensions
{
    /// <summary>
    /// Returns only the file name with its extension.
    /// </summary>
    public static string FileNameWithExtension(this IFileMetadata fm)
    {
        return string.IsNullOrWhiteSpace(fm.FileExtension)
            ? fm.FileName
            : $"{fm.FileName}.{fm.FileExtension}";
    }

    /// <summary>
    /// Returns the fully qualified output file path.
    /// </summary>
    public static string GetFilePath(this IFileMetadata fm)
    {
        return Path.GetFullPath(Path.Combine(fm.GetFullLocationPath(), fm.FileNameWithExtension()));
    }

    /// <summary>
    /// Returns the fully qualified output file path without the file extension.
    /// </summary>
    public static string GetFilePathWithoutExtension(this IFileMetadata fm)
    {
        return Path.GetFullPath(Path.Combine(fm.GetFullLocationPath(), fm.FileName));
    }
}