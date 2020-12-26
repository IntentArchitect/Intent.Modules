using System;
using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Common.Java
{
    public static class JavaOutputTargetExtensions
    {
        public static string GetPackage(this IOutputTarget target)
        {
            return string.Join(".", target.GetTargetPath().Select(x => x.Name)).Split(new [] {"java."}, StringSplitOptions.RemoveEmptyEntries).Last().ToJavaPackage();
        }
    }
}