namespace Intent.Modules.Common
{
    public class CrossPlatformPathHelpers
    {
        /// <summary>
        /// Windows accepts "\" as path seperators, but other platforms do not, so we always convert everything to "/".
        /// </summary>
        public static string NormalizePath(string path)
        {
            path = path.Replace(@"\", "/");

            while (path.Contains("//")) 
                path = path.Replace("//", "/");

            return path;
        }
    }
}
