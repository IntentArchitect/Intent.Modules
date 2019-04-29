using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentArchitectPackager = new NugetPackageInfo("Intent.IntentArchitectPackager", "1.9.2")
            .SpecifyAssetsBehaviour(new[] { "all" }, new[] { "runtime", "build", "native", "contentfiles", "analyzers", "buildtransitive" });
        public static NugetPackageInfo IntentMetadata = new NugetPackageInfo("Intent.SoftwareFactory.SDK", "1.9.0");
        public static NugetPackageInfo IntentModulesCommon = new NugetPackageInfo("Intent.Modules.Common", "1.9.0");
        public static NugetPackageInfo IntentRoslynWeaverAttributes = new NugetPackageInfo("Intent.RoslynWeaver.Attributes", "1.0.0");
    }

    public enum RegistrationType
    {
        SingleFileNoModel,
        FilePerModel,
        SingleFileListModel,
        Custom
    }
}
