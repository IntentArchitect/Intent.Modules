using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder
{
    public static class IntentNugetPackages
    {
        public static NugetPackageInfo IntentSdk = new("Intent.SoftwareFactory.SDK", "3.3.1");

        public static NugetPackageInfo IntentPackager = new NugetPackageInfo("Intent.Packager", "3.3.0")
            .SpecifyAssetsBehaviour(new[] { "all" }, new[] { "runtime", "build", "native", "contentfiles", "analyzers", "buildtransitive" });

        public static NugetPackageInfo IntentModulesCommon = new("Intent.Modules.Common", "3.3.10");
        public static NugetPackageInfo IntentModulesCommonTypes = new("Intent.Modules.Common.Types", "3.3.1");
    }
}
