using System;

namespace Intent.Modules.Angular.Templates
{
    public static class PathExtensions
    {
        public static string GetRelativePath(this string from, string to)
        {
            var url = new Uri("http://localhost/" + to, UriKind.Absolute);
            var relativeUrl = new Uri("http://localhost/" + from, UriKind.Absolute).MakeRelativeUri(url);
            return "./" + relativeUrl.ToString();
        }
    }
}