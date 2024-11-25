using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
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
            AddNugetDependency(CSharp.NugetPackages.IntentModulesCommonCSharp(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Intent.Engine")
                .AddUsing("Intent.Modules.Common.VisualStudio")
                .AddUsing("Intent.Modules.Common.CSharp.Nuget")
                .AddClass($"NugetPackages", @class =>
                {
                    @class.ImplementsInterface("INugetPackages");
                    foreach (var package in model)
                    {
                        @class.AddField("string", $"{GetPackageConstant(package)}", f => f.Private().Constant($"\"{package.Name}\""));
                    }


                    @class.AddMethod("void", "RegisterPackages", method =>
                    {
                        foreach (var package in model)
                        {
                            StringBuilder versionInfo = new StringBuilder();
                            foreach (var version in package.PackageVersions.OrderByDescending(v => GetFrameworkMajorVerion(v)))
                            {
                                var packageSettings = package.GetPackageSettings();
                                var versionNo = GetFrameworkMajorVerion(version);
                                versionInfo.AppendLine();
                                versionInfo.Append($"                        ( >= {versionNo.Major}, {versionNo.Minor}) => new PackageVersion(\"{version.Name}\"");
                                if (version.GetPackageVersionSettings()?.Locked() == true || packageSettings.Locked() == true)
                                {
                                    versionInfo.Append(", locked: true");
                                }
                                versionInfo.Append(")");
                                if (packageSettings.PrivateAssets().Any() || packageSettings.IncludeAssets().Any())
                                {
                                    var parameters = new List<string>();
                                    if (packageSettings.PrivateAssets().Any())
                                    {
                                        parameters.Add($"privateAssets: new []{{{string.Join(", ", packageSettings.PrivateAssets().Select(a => $"\"{a.Value}\""))}}}");
                                    }
                                    if (packageSettings.IncludeAssets().Any())
                                    {
                                        parameters.Add($"includeAssets: new []{{{string.Join(", ", packageSettings.IncludeAssets().Select(a => $"\"{a.Value}\""))}}}");
                                    }
                                    versionInfo.AppendLine();
                                    versionInfo.Append($"                            .SpecifyAssetsBehaviour({string.Join(",", parameters)})");
                                }
                                if (version.InternalElement.ChildElements.Any())
                                {
                                    foreach (var nugetDependencyModel in version.InternalElement.ChildElements.Where(c => c.AsNuGetDependencyModel() is not null).Select(x => x.AsNuGetDependencyModel()))
                                    {
                                        versionInfo.AppendLine();
                                        versionInfo.Append($"                            .WithNugetDependency(\"{nugetDependencyModel.Name}\", \"{nugetDependencyModel.Value}\")");
                                    }
                                }
                                versionInfo.Append(",");
                            }
                            method.AddStatement($@"NugetRegistry.Register({GetPackageConstant(package)},
                (framework) => framework switch
                    {{{versionInfo}
                        _ => throw new Exception($""Unsupported Framework `{{framework.Major}}` for NuGet package '{{{GetPackageConstant(package)}}}'""),
                    }}
                );");
                        }
                    });

                    foreach (var package in model)
                    {
                        @class.AddMethod("NugetPackageInfo", GetPackageFriendlyName(package).ToCSharpIdentifier(), method =>
                        {
                            method
                                .Static()
                                .AddParameter("IOutputTarget", "outputTarget")
                                .WithExpressionBody($"NugetRegistry.GetVersion({GetPackageConstant(package)}, outputTarget.GetMaxNetAppVersion())");
                        });
                    }

                });
        }

        private Version GetFrameworkMajorVerion(PackageVersionModel model)
        {
            if (!string.IsNullOrEmpty(model.GetPackageVersionSettings()?.MinimumTargetFramework()?.Value))
            {
                var frameworkVersion = NuGetFramework.Parse(model.GetPackageVersionSettings()?.MinimumTargetFramework().Value);
                return frameworkVersion.Version;
            }
            return new Version(0, 0);
        }

        private string GetPackageConstant(NuGetPackageModel package)
        {
            return $"{GetPackageFriendlyName(package).ToCSharpIdentifier()}PackageName";
        }

        private string GetPackageFriendlyName(NuGetPackageModel package)
        {
            if (!string.IsNullOrEmpty(package.GetPackageSettings().FriendlyName()))
            {
                return package.GetPackageSettings().FriendlyName();
            }

            return package.Name;
        }

        public override void BeforeTemplateExecution()
        {

            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: IntentModule.IntentCommonCSharp.Name,
                moduleVersion: IntentModule.IntentCommonCSharp.Version));
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && Model.Any();
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