using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementExtensionModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class ApiElementExtensionModelTemplate : CSharpTemplateBase<ElementExtensionModel>, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiElementExtensionModelTemplate(IOutputTarget outputTarget, ElementExtensionModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);

            if (Model.TypeReference.Element.Id == FolderModel.SpecializationTypeId)
            {
                AddNugetDependency(IntentNugetPackages.IntentModulesCommonTypes);
            }
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            if (Model.TypeReference.Element.Id != FolderModel.SpecializationTypeId || Model.MenuOptions == null)
            {
                return;
            }

            foreach (var creationOption in Model.MenuOptions.ElementCreations)
            {
                ExecutionContext.EventDispatcher.Publish(new NotifyModelHasParentFolderEvent(creationOption.TypeReference.Element.Id));
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

        private static string FormatForCollection(string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        private static string GetCreationOptionName(ElementCreationOptionModel option)
        {
            if (option.GetOptionSettings().ApiModelName() != null)
            {
                return option.GetOptionSettings().ApiModelName();
            }
            var name = option.Name.Replace("Add ", "").Replace("New ", "").Replace("Create ", "").ToCSharpIdentifier();
            return option.GetOptionSettings().AllowMultiple() ? name.ToPluralName() : name;
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return GetBaseElementModel().ParentModule.ApiNamespace;
        }
    }
}