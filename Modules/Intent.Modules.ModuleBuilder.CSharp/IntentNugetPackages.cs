using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder.CSharp;

public static class IntentNugetPackages
{
    public static readonly NugetPackageInfo IntentModulesCommon = new("Intent.Modules.Common", "3.6.0");
    public static readonly NugetPackageInfo IntentModulesCommonCSharp = new("Intent.Modules.Common.CSharp", "3.7.0-pre.2");
    public static readonly NugetPackageInfo IntentSoftwareFactorySdk = new("Intent.SoftwareFactory.SDK", "3.6.0");
}