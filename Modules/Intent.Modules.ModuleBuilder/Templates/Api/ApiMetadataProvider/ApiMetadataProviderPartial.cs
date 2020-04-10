using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProvider
{
    [IntentManaged(Mode.Merge)]
    partial class ApiMetadataProvider : IntentRoslynProjectItemTemplateBase<IList<ElementSettings>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiMetadataProvider";

        public ApiMetadataProvider(IProject project, IList<ElementSettings> model) : base(TemplateId, project, model)
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
                fileName: "ApiMetadataProvider",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "ApiMetadataProvider",
                @namespace: "${Project.Name}"
            );
        }

        private string GetClassName(ElementSettings elementSettings)
        {
            return GetTemplateClassName(ApiModelImplementationTemplate.ApiModelImplementationTemplate.TemplateId, elementSettings);
        }
    }
}