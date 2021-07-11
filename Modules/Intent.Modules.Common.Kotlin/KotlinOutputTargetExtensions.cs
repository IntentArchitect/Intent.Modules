using System;
using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Common.Kotlin
{
    public static class KotlinOutputTargetExtensions
    {
        public static string GetPackage(this IOutputTarget target)
        {
            return string.Join(".", target.GetTargetPath().Select(x => x.Name)).Split(new [] {"kotlin."}, StringSplitOptions.RemoveEmptyEntries).Last().ToKotlinPackage();
        }
    }
}