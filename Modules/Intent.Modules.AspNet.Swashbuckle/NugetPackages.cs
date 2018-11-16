using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNet.Swashbuckle
{
    public static class NugetPackages
    {
        public static NugetPackageInfo SwashbuckleAspNetCore = new NugetPackageInfo("Swashbuckle", "5.6.0")
            .BlockAddingOfAllFiles();
    }
}