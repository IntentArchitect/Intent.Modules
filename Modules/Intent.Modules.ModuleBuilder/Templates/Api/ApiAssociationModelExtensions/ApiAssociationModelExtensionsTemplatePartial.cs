using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiAssociationModelExtensionsTemplate : CSharpTemplateBase<AssociationSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiAssociationModelExtensionsTemplate(IOutputTarget outputTarget, AssociationSettingsModel model) : base(TemplateId, outputTarget, model)
        {
            foreach (var sourceTypes in Model.SourceEnd.TargetTypes().Select(x => x.AsElementSettingsModel()).Where(x => x != null))
            {
                if (!string.IsNullOrWhiteSpace(sourceTypes.ParentModule.NuGetPackageId) &&
                    outputTarget.GetProject().Name != sourceTypes.ParentModule.NuGetPackageId)
                {
                    AddNugetDependency(new NugetPackageInfo(sourceTypes.ParentModule.NuGetPackageId, sourceTypes.ParentModule.NuGetPackageVersion));
                }
            }
            foreach (var targetType in Model.TargetEnd.TargetTypes().Select(x => x.AsElementSettingsModel()).Where(x => x != null))
            {
                if (!string.IsNullOrWhiteSpace(targetType.ParentModule.NuGetPackageId) &&
                    outputTarget.GetProject().Name != targetType.ParentModule.NuGetPackageId)
                {
                    AddNugetDependency(new NugetPackageInfo(targetType.ParentModule.NuGetPackageId, targetType.ParentModule.NuGetPackageVersion));
                }
            }
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}AssociationExtensions",
                @namespace: Model.ParentModule.ApiNamespace);
        }
    }
}