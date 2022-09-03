using System;
using System.IO;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    /// <summary>
    /// Obsolete. Use <see cref="FileMetadataExtensions"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static class IFileMetadataExtensions
    {
        /// <summary>
        /// Obsolete. Use <see cref="FileMetadataExtensions.FileNameWithExtension"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string FileNameWithExtension(IFileMetadata fm)
        {
            return string.IsNullOrWhiteSpace(fm.FileExtension)
                ? fm.FileName
                : $"{fm.FileName}.{fm.FileExtension}";
        }

        /// <summary>
        /// Use <see cref="GetFilePath"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string GetFullLocationPathWithFileName(IFileMetadata fm)
        {
            return Path.GetFullPath(Path.Combine(fm.GetFullLocationPath(), fm.FileNameWithExtension()));
        }

        /// <summary>
        /// Obsolete. Use <see cref="FileMetadataExtensions.GetFilePath"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string GetFilePath(IFileMetadata fm)
        {
            return Path.GetFullPath(Path.Combine(fm.GetFullLocationPath(), fm.FileNameWithExtension()));
        }

        /// <summary>
        /// Obsolete. Use <see cref="FileMetadataExtensions.GetFilePathWithoutExtension"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string GetFilePathWithoutExtension(IFileMetadata fm)
        {
            return Path.Combine(fm.GetFullLocationPath(), fm.FileName);
        }
    }
}
