using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Unity
{
    public class NugetPackages
    {
        public static NugetPackageInfo Unity = new NugetPackageInfo("Unity", "5.8.6")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Unity.Abstractions", "3.3.0.0", "6d32ff45e0ccc69f"))
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo UnityWebApi = new NugetPackageInfo("Unity.WebAPI", "5.3.0")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo UnityAspNetWebApi = new NugetPackageInfo("Unity.AspNet.WebApi", "4.0.1")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo IntentFrameworkCore = new NugetPackageInfo("Intent.Framework.Core", "1.0.0");

    }
}