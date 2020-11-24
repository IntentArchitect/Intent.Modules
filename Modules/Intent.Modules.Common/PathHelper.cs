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
#if NETCOREAPP2_1
            return Path.GetRelativePath(relativeTo.NormalizePath(), path.NormalizePath());
#endif
#if NETSTANDARD2_1
            return Path.GetRelativePath(relativeTo.NormalizePath(), path.NormalizePath());
#endif
            if (Path.HasExtension(relativeTo))
            {
                relativeTo = Path.GetDirectoryName(relativeTo);
            }

            // Require trailing backslash for path
            if (!relativeTo.EndsWith("\\") && !relativeTo.EndsWith("/"))
                relativeTo += "/";

            Uri baseUri = new Uri(relativeTo);
            Uri fullUri = new Uri(path);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            return relativeUri.ToString().Replace("%20", " ");
        }
    }
}
