using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder.CSharp;

public static class IntentNugetPackages
{
    public static readonly NugetPackageInfo IntentModulesBlazor = new("Intent.Modules.Blazor", "1.0.0-alpha.0");
    public static readonly NugetPackageInfo IntentModulesCommon = new("Intent.Modules.Common", "3.6.0");
    public static readonly NugetPackageInfo IntentModulesCommonCSharp = new("Intent.Modules.Common.CSharp", "3.7.0-pre.1");
    public static readonly NugetPackageInfo IntentSoftwareFactorySdk = new("Intent.SoftwareFactory.SDK", "3.6.0");
}