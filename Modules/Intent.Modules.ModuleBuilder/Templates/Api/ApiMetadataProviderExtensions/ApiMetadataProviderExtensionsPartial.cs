using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions
{
    [IntentManaged(Mode.Merge)]
    partial class ApiMetadataProviderExtensions : IntentRoslynProjectItemTemplateBase<IList<ElementSettingsModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions";

        public ApiMetadataProviderExtensions(IProject project, IList<ElementSettingsModel> model) : base(TemplateId, project, model)
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
                fileName: "ApiMetadataProviderExtensions",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "ApiMetadataProviderExtensions",
                @namespace: "${Project.Name}"
            );
        }

        public string MetadataProviderClassName => GetTemplateClassName(ApiMetadataProvider.ApiMetadataProvider.TemplateId);

        private string GetClassName(ElementSettingsModel elementSettings)
        {
            return GetTemplateClassName(ApiModelImplementationTemplate.ApiModelImplementationTemplate.TemplateId, elementSettings);
        }
    }
}