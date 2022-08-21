using System;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java
{
    public static class TemplateExtensions
    {
        /// <summary>
        /// This should not be used. A package structure is determined by the folders in the Intent
        /// Architect designers from which the Software Factory generates a folder structure on the
        /// file system. To avoid unexpected differences between what is modeled in designers and
        /// what is generated, folders are generated exactly as is.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string ToJavaPackage(this string s) => s;

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
