using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ApplicationTemplate.Builder
{
    public static class IntentNugetPackages
    {
        public static NugetPackageInfo IntentPackager = new NugetPackageInfo("Intent.Packager", "3.0.4")
            .SpecifyAssetsBehaviour(new[] { "all" }, new[] { "runtime", "build", "native", "contentfiles", "analyzers", "buildtransitive" });
    }
}
