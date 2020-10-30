using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiAssociationModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiAssociationModel : CSharpTemplateBase<AssociationSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiAssociationModel";

        public ApiAssociationModel(IOutputTarget project, AssociationSettingsModel model) : base(TemplateId, project, model)
        {
        }

        public string AssociationEndClassName => $"{Model.Name.ToCSharpIdentifier()}EndModel";
        public string AssociationSourceEndClassName => $"{Model.SourceEnd.Name.ToCSharpIdentifier()}Model";
        public string AssociationTargetEndClassName => $"{Model.TargetEnd.Name.ToCSharpIdentifier()}Model";

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: Model.ParentModule.ApiNamespace);
        }
    }
}