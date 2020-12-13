using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiAssociationModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiAssociationModelTemplate : CSharpTemplateBase<AssociationSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiAssociationModel";

        public ApiAssociationModelTemplate(IOutputTarget project, AssociationSettingsModel model) : base(TemplateId, project, model)
        {
        }

        public string AssociationEndClassName => $"{Model.Name.ToCSharpIdentifier()}EndModel";
        public string AssociationSourceEndClassName => $"{Model.SourceEnd.Name.ToCSharpIdentifier()}Model";
        public string AssociationTargetEndClassName => $"{Model.TargetEnd.Name.ToCSharpIdentifier()}Model";

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: Model.ParentModule.ApiNamespace);
        }
    }
}