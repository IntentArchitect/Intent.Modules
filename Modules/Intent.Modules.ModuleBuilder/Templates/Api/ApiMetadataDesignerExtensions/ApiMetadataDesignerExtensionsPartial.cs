using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiMetadataDesignerExtensions : IntentRoslynProjectItemTemplateBase<IList<DesignerModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions";

        public ApiMetadataDesignerExtensions(IProject project, IList<DesignerModel> model) : base(TemplateId, project, model)
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
                fileName: "ApiMetadataDesignerExtensions",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "ApiMetadataDesignerExtensions",
                @namespace: "${Project.Name}"
            );
        }

    }
}