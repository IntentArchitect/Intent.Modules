using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementExtensionModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiElementExtensionModelTemplate : CSharpTemplateBase<ElementExtensionModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiElementExtensionModel";

        public ApiElementExtensionModelTemplate(IOutputTarget project, ElementExtensionModel model) : base(TemplateId, project, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            if (Model.TypeReference.Element.Id == FolderModel.SpecializationTypeId && Model.MenuOptions != null)
            {
                foreach (var creationOption in Model.MenuOptions.ElementCreations)
                {
                    ExecutionContext.EventDispatcher.Publish(new NotifyModelHasParentFolderEvent(creationOption.TypeReference.Element.Id));
                }
            }
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: Model.ParentModule.ApiNamespace);
        }

        private ElementSettingsModel GetBaseElementModel()
        {
            return new ElementSettingsModel((IElement)Model.TypeReference.Element);
        }

        private string FormatForCollection(string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        private string GetCreationOptionName(ElementCreationOptionModel option)
        {
            if (option.GetOptionSettings().ApiModelName() != null)
            {
                return option.GetOptionSettings().ApiModelName();
            }
            var name = option.Name.Replace("Add ", "").Replace("New ", "").Replace("Create ", "").ToCSharpIdentifier();
            return option.GetOptionSettings().AllowMultiple() ? name.ToPluralName() : name;
        }
    }
}