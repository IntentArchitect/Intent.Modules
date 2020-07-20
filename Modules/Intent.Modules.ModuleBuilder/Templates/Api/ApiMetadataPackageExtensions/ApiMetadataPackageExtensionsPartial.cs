using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiMetadataPackageExtensions : IntentRoslynProjectItemTemplateBase<IList<PackageSettingsModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions";

        public ApiMetadataPackageExtensions(IProject project, IList<PackageSettingsModel> model) : base(TemplateId, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "ApiMetadataPackageExtensions",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "ApiMetadataPackageExtensions",
                @namespace: "${Project.Name}"
            );
        }

        private string GetClassName(PackageSettingsModel packageSettings)
        {
            return GetTemplateClassName(ApiPackageModel.ApiPackageModel.TemplateId, packageSettings);
        }

    }
}