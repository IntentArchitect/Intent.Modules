using System;
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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiAssociationModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiAssociationModelTemplate : CSharpTemplateBase<AssociationSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiAssociationModel";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiAssociationModelTemplate(IOutputTarget outputTarget, AssociationSettingsModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public string AssociationEndClassName => $"{Model.Name.ToCSharpIdentifier()}EndModel";
        public string AssociationSourceEndClassName => $"{Model.SourceEnd.Name.ToCSharpIdentifier()}Model";
        public string AssociationTargetEndClassName => $"{Model.TargetEnd.Name.ToCSharpIdentifier()}Model";

        public string GetSourceEndInterfaces()
        {
            var interfaces = new List<string>();
            foreach (var traitStereotype in Model.SourceEnd.Stereotypes.Where(x => x.Definition.IsTrait))
            {
                interfaces.Add($"I{traitStereotype.Name.ToCSharpIdentifier()}Model");
            }

            return interfaces.Any() ? $", {string.Join(", ", interfaces)}" : "";
        }

        public string GetTargetEndInterfaces()
        {
            var interfaces = new List<string>();
            foreach (var traitStereotype in Model.TargetEnd.Stereotypes.Where(x => x.Definition.IsTrait))
            {
                interfaces.Add($"I{traitStereotype.Name.ToCSharpIdentifier()}Model");
            }

            return interfaces.Any() ? $", {string.Join(", ", interfaces)}" : "";
        }

        private static string FormatForCollection(string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: Model.ParentModule.ApiNamespace);
        }
    }
}