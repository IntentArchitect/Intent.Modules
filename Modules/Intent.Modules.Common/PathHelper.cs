using System;

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
            return SystemPath.GetRelativePath(relativeTo.NormalizePath(), path.NormalizePath());
#endif
#if NETSTANDARD2_1
            return SystemPath.GetRelativePath(relativeTo.NormalizePath(), path.NormalizePath());
#endif
            var url = new Uri("http://localhost/" + path, UriKind.Absolute);
            var relativeUrl = new Uri("http://localhost/" + relativeTo, UriKind.Absolute).MakeRelativeUri(url);
            return "./" + relativeUrl.ToString();
            //throw new NotSupportedException("This is only supported if the entry executable is at least .NET Core 2.1 or higher.");
        }
    }
}
