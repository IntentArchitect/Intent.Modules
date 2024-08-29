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
            AddNugetDependency(IntentNugetPackages.IntentModulesCommonCSharp);

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
                                var majorFrameworkVersion = GetFrameworkMajorVerion(version);
                                versionInfo.AppendLine();
                                versionInfo.Append($"                        ( >= {majorFrameworkVersion}, 0) => new PackageVersion(\"{version.Name}\"");
                                if (version.GetPackageVersionSettings()?.Locked() == true || package.GetPackageSettings()?.Locked() == true)
                                {
                                    versionInfo.Append(", locked: true");
                                }
                                versionInfo.Append("),");
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

        private int GetFrameworkMajorVerion(PackageVersionModel model)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(model.GetPackageVersionSettings()?.MinimumTargetFramework()?.Value))
            {
                var frameworkVersion = NuGetFramework.Parse(model.GetPackageVersionSettings()?.MinimumTargetFramework().Value);
                result = frameworkVersion.Version.Major;
            }
            return result;
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