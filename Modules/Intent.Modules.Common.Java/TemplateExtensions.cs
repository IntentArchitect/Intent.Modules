using System.Linq;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java
{
    public static class TemplateExtensions
    {
        public static string ToJavaPackage(this string s)
        {
            return string.Concat(s.Split(' ')
                        .Select(x => string.Join("_", x.Split('-').Select(p => p)))
                        .Select(x => string.Join(".", x.Split('.').Select(p => p.ToSnakeCase()))))
                    .Replace("#", "Sharp")
                    .Replace("&", "And")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(",", "")
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("{", "")
                    .Replace("}", "")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace("?", "")
                    .Replace("@", "")
                ;
        }

        public static string ToJavaIdentifier(this string s)
        {
            return string.Concat(s.Split(' ').SelectMany(x => x.Split('-')).Select(x => x.ToPascalCase()))
                .Replace("#", "Sharp")
                .Replace("&", "And")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(",", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace(".", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace("?", "")
                .Replace("@", "")
                ;
        }

    }

}
