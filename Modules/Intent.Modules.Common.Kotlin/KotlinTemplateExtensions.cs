using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Intent.Engine;
using Intent.Modules.Common.Kotlin.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.Kotlin
{
    public static class KotlinTemplateExtensions
    {
        /// <summary>
        /// Returns the package name based on the <paramref name="template"/>'s Output Target and folder location within the designer.
        /// Uses the 'kotlin' folder as the root for the package name.
        /// </summary>
        /// <param name="template"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPackageName<T>(this KotlinTemplateBase<T> template)
        {
            return GetPackageName(template, "kotlin");
        }

        /// <summary>
        /// Returns the package name based on the <paramref name="template"/>'s Output Target and folder location within the designer.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="rootFolder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPackageName<T>(this KotlinTemplateBase<T> template, string rootFolder)
        {
            {
                if (template.Model is IHasFolder model)
                {
                    return string.Join(".", new[] { template.OutputTarget.GetPackageName(rootFolder) }.Concat(model.GetParentFolderNames())).ToKotlinPackage();
                }
            }
            {
                if (template.Model is IHasFolder<IFolder> model)
                {
                    return string.Join(".", new [] { template.OutputTarget.GetPackageName(rootFolder) }.Concat(model.GetParentFolderNames())).ToKotlinPackage();
                }
            }
            return template.OutputTarget.GetPackageName(rootFolder);
        }

        /// <summary>
        /// Returns the package name of the <paramref name="target"/>'s using the 'kotlin' folder as the root.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string GetPackageName(this IOutputTarget target)
        {
            return target.GetPackageName("kotlin");
        }

        /// <summary>
        /// Returns the package name of the <paramref name="target"/>'s using the <paramref name="rootFolder"/> folder as the root.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rootFolder"></param>
        /// <returns></returns>
        public static string GetPackageName(this IOutputTarget target, string rootFolder)
        {
            return string.Join(".", target.GetTargetPath().Select(x => x.Name)
                .Reverse()
                .TakeWhile(x => !string.Equals(x, rootFolder, StringComparison.InvariantCultureIgnoreCase))
                .Reverse())
                .ToKotlinPackage();
        }

        /// <summary>
        /// Escapes any invalid characters in the string for a Kotlin package name.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToKotlinPackage(this string s)
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

        /// <summary>
        /// Escapes any invalid characters in the string for a Kotlin identifier name (e.g. class, interface, etc.).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToKotlinIdentifier(this string s)
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
