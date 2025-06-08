using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder
{
    public static class IntentNugetPackages
    {
        public static NugetPackageInfo IntentSdk = new("Intent.SoftwareFactory.SDK", "3.7.0");
        public static NugetPackageInfo IntentPersistenceSdk = new("Intent.Persistence.SDK", "1.0.1-alpha.1");

        public static NugetPackageInfo IntentPackager = new NugetPackageInfo("Intent.Packager", "3.6.0-pre.1")
            .SpecifyAssetsBehaviour(new[] { "all" }, new[] { "runtime", "build", "native", "contentfiles", "analyzers", "buildtransitive" });

        public static NugetPackageInfo IntentModulesCommon = new("Intent.Modules.Common", "3.7.2");
        public static NugetPackageInfo IntentModulesCommonTypes = new("Intent.Modules.Common.Types", "3.4.0");
    }
}
