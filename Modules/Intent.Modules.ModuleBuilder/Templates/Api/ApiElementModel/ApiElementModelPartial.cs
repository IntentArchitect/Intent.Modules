using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiElementModel : IntentRoslynProjectItemTemplateBase<ElementSettingsModel>
    {
        public List<AssociationSettingsModel> AssociationSettings { get; }

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiElementModel";

        public ApiElementModel(IProject project, ElementSettingsModel model, List<AssociationSettingsModel> associationSettings) : base(TemplateId, project, model)
        {
            AssociationSettings = associationSettings;
            AddTypeSource(CSharpTypeSource.InProject(Project, ApiElementModel.TemplateId, collectionFormat: "IEnumerable<{0}>"));
            AddNugetDependency(NugetPackages.IntentSdk);
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
                fileName: $"{Model.ApiModelName}",
                fileExtension: "cs",
                defaultLocationInProject: "Api",
                className: $"{Model.ApiModelName}",
                @namespace: Model.Designer.ApiNamespace
            );
        }

        public string BaseType => Model.GetInheritedType() != null
            ? GetTemplateClassName(TemplateId, Model.GetInheritedType().Id, throwIfNotFound: false)
                ?? $"{Model.GetInheritedType().Name.ToCSharpIdentifier()}Model"
            : null;


        private string FormatForCollection(string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        private string FormatForCollection(AssociationCreationOptionModel option)
        {
            var asCollection = option.GetOptionSettings().AllowMultiple();
            return asCollection ? $"IList<{option.Type.ApiModelName}>" : option.Type.ApiModelName;
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

        private bool ExistsInBase(AssociationCreationOptionModel creationOption)
        {
            return Model.GetInheritedType()?.MenuOptions.AssociationCreations.Any(x => x.Type.Id == creationOption.Type.Id) ??
                   false;
        }

        private bool ExistsInBase(AssociationSettingsModel associationSettings)
        {
            return false;
            //throw new System.NotImplementedException();
        }
    }
}