using System.IO;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public static class IFileMetadataExtensions
    {
        public static string FileNameWithExtension(this IFileMetadata fm)
        {
            return $"{fm.FileName}.{fm.FileExtension}";
        }

        public static string GetFullLocationPathWithFileName(this IFileMetadata fm)
        {
            return Path.Combine(fm.GetFullLocationPath(), fm.FileNameWithExtension());
        }

        public static string GetRelativeFilePathWithFileName(this IFileMetadata fm)
        {
            var relativePath = fm.GetRelativeFilePath();
            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                return CrossPlatformPathHelpers.NormalizePath(Path.Combine(relativePath, fm.FileName));
            }
            return fm.FileNameWithExtension();
        }

        public static string GetRelativeFilePathWithFileNameWithExtension(this IFileMetadata fm)
        {
            string relativePath = fm.GetRelativeFilePath();
            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                return CrossPlatformPathHelpers.NormalizePath(Path.Combine(relativePath, fm.FileNameWithExtension()));
            }

            return fm.FileNameWithExtension();
        }

    }
}
