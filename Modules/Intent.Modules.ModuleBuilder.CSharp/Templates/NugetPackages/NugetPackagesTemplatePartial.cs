using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using NuGet.Frameworks;
using NuGet.Packaging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.NugetPackages
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NugetPackagesTemplate : CSharpTemplateBase<IList<NuGetPackageModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.CSharp.Templates.NugetPackages";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NugetPackagesTemplate(IOutputTarget outputTarget, IList<NuGetPackageModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Intent.Engine")
                .AddUsing("Intent.Modules.Common.CSharp.VisualStudio")
                .AddUsing("Intent.Modules.Common.CSharp.Nuget")
                .AddClass($"NugetPackages", @class =>
                {
                    foreach (var package in model)
                    {
                        @class.AddField("string", $"{GetPackageConstant(package)}", f => f.Private().Constant($"\"{GetPackageName(package)}\""));
                    }

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Static();
                        foreach (var package in model)
                        {
                            StringBuilder versionInfo = new StringBuilder();
                            foreach (var version in package.PackageVersions)
                            {
                                var majorFrameworkVersion = 0;
                                if (!string.IsNullOrEmpty(version.GetPackageVersionSettings()?.MinimumTargetFramework()?.Value))
                                {
                                    var frameworkVersion = NuGetFramework.Parse(version.GetPackageVersionSettings()?.MinimumTargetFramework().Value);
                                    majorFrameworkVersion = frameworkVersion.Version.Major;
                                }
                                versionInfo.AppendLine();
                                versionInfo.Append($"                        ( >= {majorFrameworkVersion}, 0) => new PackageVersion(\"{version.Name}\"");
                                if (version.GetPackageVersionSettings()?.Locked() == true)
                                {
                                    versionInfo.Append(", locked: true");
                                }
                                versionInfo.Append("),");
                            }
                            ctor.AddStatement($@"NugetRegistry.Register({GetPackageConstant(package)},
                (framework) => framework switch
                    {{{versionInfo}
                        _ => throw new Exception($""Unsupported Framework `{{framework.Major}}` for NuGet package '{{{GetPackageConstant(package)}}}'""),
                    }}
                );");
                        }
                    });

                    foreach (var package in model)
                    {
                        @class.AddMethod("NugetPackageInfo", package.Name.ToCSharpIdentifier(), method =>
                        {
                            method
                                .Static()
                                .AddParameter("IOutputTarget", "outputTarget")
                                .WithExpressionBody($"NugetRegistry.GetVersion({GetPackageConstant(package)}, outputTarget.GetMaxNetAppVersion())");
                        });
                    }

                });
        }

        private string GetPackageConstant(NuGetPackageModel package)
        {
            return $"{package.Name.ToCSharpIdentifier()}PackageName";
        }

        private string GetPackageName(NuGetPackageModel package)
        {
            if (!string.IsNullOrEmpty(package.GetPackageSettings().FriendlyName()))
            {
                return package.GetPackageSettings().FriendlyName();
            }

            return package.Name;
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}