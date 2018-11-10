using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Interop.Autofac
{
    public static class NugetPackages
    {
        public static NugetPackageInfo AutofacWebApi2Owin = new NugetPackageInfo("Autofac.WebApi2.Owin", "4.0.0")
            .BlockAddingOfAllFiles();
    }
}
