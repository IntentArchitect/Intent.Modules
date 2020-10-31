using System;
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

        [Obsolete("Use GetFilePath")]
        public static string GetFullLocationPathWithFileName(this IFileMetadata fm)
        {
            return Path.Combine(fm.GetFullLocationPath(), fm.FileNameWithExtension());
        }

        public static string GetFilePath(this IFileMetadata fm)
        {
            return Path.Combine(fm.GetFullLocationPath(), fm.FileNameWithExtension());
        }

        public static string GetFilePathWithoutExtension(this IFileMetadata fm)
        {
            return Path.Combine(fm.GetFullLocationPath(), fm.FileName);
        }
    }
}
