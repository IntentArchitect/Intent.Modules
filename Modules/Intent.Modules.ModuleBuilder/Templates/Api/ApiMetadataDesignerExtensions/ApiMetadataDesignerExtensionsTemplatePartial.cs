using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiMetadataDesignerExtensionsTemplate : CSharpTemplateBase<IList<DesignerModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiMetadataDesignerExtensionsTemplate(IOutputTarget outputTarget, IList<DesignerModel> model) : base(TemplateId, outputTarget, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "ApiMetadataDesignerExtensions",
                @namespace: Model.First().ParentModule.ApiNamespace);
        }
    }
}