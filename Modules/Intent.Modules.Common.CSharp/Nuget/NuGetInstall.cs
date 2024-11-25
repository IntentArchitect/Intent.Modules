using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.CSharp.Nuget
{
    public class NuGetInstall
    {

        public NuGetInstall(INugetPackageInfo package) : this (package, new NuGetInstallOptions())
        {
        }

        public NuGetInstall(INugetPackageInfo package, NuGetInstallOptions options)
        {
            Options = options;
            Package = package;
        }

        public NuGetInstallOptions Options { get; }
        public INugetPackageInfo Package { get; }
    }
}
