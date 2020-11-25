using System;
using System.IO;

namespace Intent.Modules.Common
{
    public static class PathHelper
    {
        /// <summary>
        /// Windows accepts "\" as path seperators, but other platforms do not, so we always convert everything to "/".
        /// </summary>
        public static string NormalizePath(this string path)
        {
            path = path.Replace(@"\", "/");

            while (path.Contains("//")) 
                path = path.Replace("//", "/");

            return path;
        }

        public static string GetRelativePath(this string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo.NormalizePath(), path.NormalizePath()).NormalizePath();
        }
    }
}
