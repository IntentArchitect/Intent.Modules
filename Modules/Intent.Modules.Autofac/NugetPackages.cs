using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Autofac
{
    public class NugetPackages
    {
        public static NugetPackageInfo AutofacExtensionsDependencyInjection = new NugetPackageInfo("Autofac.Extensions.DependencyInjection", "4.3.1")
            .BlockAddingOfAllFiles();
    }
}