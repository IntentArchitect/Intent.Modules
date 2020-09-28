using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentArchitectPackager = new NugetPackageInfo("Intent.IntentArchitectPackager", "2.2.0-pre.1")
            .SpecifyAssetsBehaviour(new[] { "all" }, new[] { "runtime", "build", "native", "contentfiles", "analyzers", "buildtransitive" });
        public static NugetPackageInfo IntentSdk = new NugetPackageInfo("Intent.SoftwareFactory.SDK", "3.0.0-pre.4");
        public static NugetPackageInfo IntentModulesCommon = new NugetPackageInfo("Intent.Modules.Common", "3.0.0-pre.1");
    }
}
