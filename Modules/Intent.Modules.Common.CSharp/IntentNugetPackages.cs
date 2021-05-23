using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Common.CSharp
{
    public static class IntentNugetPackages
    {
        public static NugetPackageInfo IntentRoslynWeaverAttributes = new NugetPackageInfo("Intent.RoslynWeaver.Attributes", "3.0.2");
        public static NugetPackageInfo IntentSdk = new NugetPackageInfo("Intent.SoftwareFactory.SDK", "3.0.0");

        public static NugetPackageInfo IntentPackager = new NugetPackageInfo("Intent.Packager", "3.0.1")
            .SpecifyAssetsBehaviour(new[] { "all" }, new[] { "runtime", "build", "native", "contentfiles", "analyzers", "buildtransitive" });

        public static NugetPackageInfo IntentModulesCommon = new NugetPackageInfo("Intent.Modules.Common", "3.0.6");
        public static NugetPackageInfo IntentModulesCommonTypes = new NugetPackageInfo("Intent.Modules.Common.Types", "3.0.5");
        public static NugetPackageInfo IntentCommonCSharp = new NugetPackageInfo("Intent.Modules.Common.CSharp", "3.0.10");
        public static NugetPackageInfo IntentCommonSql = new NugetPackageInfo("Intent.Modules.Common.Sql", "3.0.4");
        public static NugetPackageInfo IntentCommonHtml = new NugetPackageInfo("Intent.Modules.Common.Html", "3.0.2");
        public static NugetPackageInfo IntentCommonTypescript = new NugetPackageInfo("Intent.Modules.Common.TypeScript", "3.0.7");
        public static NugetPackageInfo IntentCommonJava = new NugetPackageInfo("Intent.Modules.Common.Java", "3.0.5");
    }
}
