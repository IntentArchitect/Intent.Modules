using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Unity
{
    public class NugetPackages
    {
        public static NugetPackageInfo Unity = new NugetPackageInfo("Unity", "4.0.1", "net45")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo UnityWebApi = new NugetPackageInfo("Unity.WebAPI", "5.2.3", "net452")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo UnityAspNetWebApi = new NugetPackageInfo("Unity.AspNet.WebApi", "4.0.1", "net452")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo IntentFrameworkUnity = new NugetPackageInfo("Intent.Framework.Unity", "0.1.2-beta", "net45");

    }
}