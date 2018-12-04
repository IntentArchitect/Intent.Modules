using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Interop.Autofac
{
    public static class NugetPackages
    {
        public static NugetPackageInfo AutofacOwin = new NugetPackageInfo("Autofac.Owin", "4.2.0")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Autofac.Integration.Owin", "4.2.0.0", "17863af14b0044da"))
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo AutofacWebApi2 = new NugetPackageInfo("Autofac.WebApi2", "4.2.0")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Autofac.Integration.WebApi", "4.2.0.0", "17863af14b0044da"))
            .BlockAddingOfAllFiles();
        public static NugetPackageInfo AutofacWebApi2Owin = new NugetPackageInfo("Autofac.WebApi2.Owin", "4.0.0")
            .BlockAddingOfAllFiles();
    }
}
