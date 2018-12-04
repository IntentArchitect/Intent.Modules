using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.Swashbuckle
{
    public static class NugetPackages
    {
        public static NugetPackageInfo Swashbuckle = new NugetPackageInfo("Swashbuckle", "5.6.0")
            .BlockAddingOfAllFiles();
    }
}