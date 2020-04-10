using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelExtensions
{
    [IntentManaged(Mode.Merge)]
    partial class ApiModelExtensions : IntentRoslynProjectItemTemplateBase<ElementSettings>
    {
        protected readonly List<IStereotypeDefinition> StereotypeDefinitions;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiModelExtensions";

        public ApiModelExtensions(IProject project, ElementSettings model, List<IStereotypeDefinition> stereotypeDefinitions) : base(TemplateId, project, model)
        {
            StereotypeDefinitions = stereotypeDefinitions;
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
                fileName: $"{Model.Name.ToCSharpIdentifier()}Extensions",
                fileExtension: "cs",
                defaultLocationInProject: "Api/Extensions",
                className: $"{Model.Name.ToCSharpIdentifier()}Extensions",
                @namespace: Model.Modeler.ApiNamespace
            );
        }

        public string ModelClassName => GetTemplateClassName(ApiModelImplementationTemplate.ApiModelImplementationTemplate.TemplateId, Model);
    }
}