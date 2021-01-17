using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public static class OutputTargetExtensions
    {
        public static IOutputTarget GetProject(this IOutputTarget outputTarget)
        {
            return outputTarget.GetTargetPath()[0];
        }
    }
}
