using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public class RemoveNugetPackageEvent
    {
        public string NugetPackageName { get; }
        public IOutputTarget Target { get; }

        public RemoveNugetPackageEvent(string nugetPackageName, IOutputTarget target)
        {
            NugetPackageName = nugetPackageName;
            Target = target;
        }
    }
}
