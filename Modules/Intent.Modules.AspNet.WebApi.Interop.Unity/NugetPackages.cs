using Intent.SoftwareFactory.VisualStudio;
using System;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity
{
    public static class NugetPackages
    {
        public static NugetPackageInfo UnityWebApi = new NugetPackageInfo("Unity.WebAPI", "5.3.0", "net452")
            .BlockAddingOfAllFiles();
    }
}
