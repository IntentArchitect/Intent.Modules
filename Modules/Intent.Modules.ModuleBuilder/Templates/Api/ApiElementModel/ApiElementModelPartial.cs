using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiElementModel : IntentRoslynProjectItemTemplateBase<ElementSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiElementModel";

        public ApiElementModel(IProject project, ElementSettingsModel model) : base(TemplateId, project, model)
        {
            AddTypeSource(CSharpTypeSource.InProject(Project, ApiElementModel.TemplateId, collectionFormat: "IEnumerable<{0}>"));
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
                fileName: $"{Model.ApiClassName}",
                fileExtension: "cs",
                defaultLocationInProject: "Api",
                className: $"{Model.ApiClassName}",
                @namespace: Model.Designer.ApiNamespace
            );
        }

        public string BaseType => Model.GetInheritedType() != null
            ? GetTemplateClassName(TemplateId, Model.GetInheritedType().Id, throwIfNotFound: false)
                ?? $"{Model.GetInheritedType().Name.ToCSharpIdentifier()}Model"
            : null;


        private string GetCreationOptionTypeClass(ElementCreationOptionModel option, bool asCollection = false)
        {
            var @interface = GetTemplateClassName(ApiElementModel.TemplateId, option.Type.Id, throwIfNotFound: false) ?? option.Type.ApiClassName;
            return asCollection ? $"IList<{@interface}>" : @interface;
        }

        private string GetCreationOptionName(ElementCreationOptionModel option)
        {
            if (option.GetOptionSettings().ApiModelName() != null)
            {
                return option.GetOptionSettings().ApiModelName();
            }
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.GetOptionSettings().AllowMultiple() ? name.ToPluralName() : name;
        }

        private bool ExistsInBase(ElementCreationOptionModel creationOption)
        {
            return Model.GetInheritedType()?.MenuOptions.ElementCreations.Any(x => x.Type.Id == creationOption.Type.Id) ??
                   false;
        }
    }
}