using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp
{
    public class NugetPackages : INugetPackages
    {
        public const string IntentModulesCommonCSharpPackageName = "Intent.Modules.Common.CSharp";

        public void RegisterPackages()
        {
            NugetRegistry.Register(IntentModulesCommonCSharpPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("3.8.0-pre.7")
                            .WithNugetDependency("Intent.Architect.Persistence", "3.6.0")
                            .WithNugetDependency("Intent.Modules.Common", "3.7.0-pre.3")
                            .WithNugetDependency("Intent.Modules.Common.Types", "4.0.0-alpha.0")
                            .WithNugetDependency("Intent.RoslynWeaver.Attributes", "2.1.4")
                            .WithNugetDependency("Intent.SoftwareFactory.SDK", "3.6.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IntentModulesCommonCSharpPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo IntentModulesCommonCSharp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IntentModulesCommonCSharpPackageName, outputTarget.GetMaxNetAppVersion());
    }
}