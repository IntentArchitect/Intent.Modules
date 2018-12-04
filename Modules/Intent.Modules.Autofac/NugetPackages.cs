using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Autofac
{
    public class NugetPackages
    {
        public static NugetPackageInfo Autofac = new NugetPackageInfo("Autofac", "4.8.1")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Autofac", "4.8.1.0", "17863af14b0044da"))
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo AutofacExtensionsDependencyInjection = new NugetPackageInfo("Autofac.Extensions.DependencyInjection", "4.3.1")
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo MicrosoftExtensionsDependencyInjectionAbstractions = new NugetPackageInfo("Microsoft.Extensions.DependencyInjection.Abstractions", "1.1.0")
            .BlockAddingOfAllFiles();
    }
}