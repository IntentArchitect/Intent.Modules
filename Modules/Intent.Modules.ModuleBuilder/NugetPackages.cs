using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder
{
    public static class NugetPackages
    {
        public static NugetPackageInfo IntentArchitectPackager = new NugetPackageInfo("Intent.IntentArchitectPackager", "1.3.0");
        public static NugetPackageInfo IntentMetadata = new NugetPackageInfo("Intent.SoftwareFactory.SDK", "1.7.0");
        public static NugetPackageInfo IntentModulesCommon = new NugetPackageInfo("Intent.Modules.Common", "1.7.0");
        public static NugetPackageInfo IntentRoslynWeaverAttributes = new NugetPackageInfo("Intent.RoslynWeaver.Attributes", "1.0.0");
    }
}
