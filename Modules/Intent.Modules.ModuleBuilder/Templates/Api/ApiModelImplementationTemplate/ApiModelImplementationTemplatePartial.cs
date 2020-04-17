using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelImplementationTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiModelImplementationTemplate : IntentRoslynProjectItemTemplateBase<ElementSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiModelImplementationTemplate";

        public ApiModelImplementationTemplate(IProject project, ElementSettingsModel model) : base(TemplateId, project, model)
        {
            AddTypeSource(CSharpTypeSource.InProject(Project, ApiModelImplementationTemplate.TemplateId, collectionFormat: "IEnumerable<{0}>"));
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


        private string GetCreationOptionTypeInterface(CreationOptionModel option, bool asCollection)
        {
            var @interface = GetTemplateClassName(ApiModelImplementationTemplate.TemplateId, option.Type.Id, throwIfNotFound: false);
            if (@interface == null)
            {
                return null;
            }
            return asCollection ? $"IList<{@interface}>" : @interface;
        }

        private string GetCreationOptionTypeClass(CreationOptionModel option)
        {
            var className = GetTemplateClassName(TemplateId, option.Type.Id, throwIfNotFound: false);
            return className;
        }

        private string GetCreationOptionName(CreationOptionModel option)
        {
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.GetOptionSettings().AllowMultiple() ? name.ToPluralName() : name;
        }

        private bool ExistsInBase(CreationOptionModel creationOption)
        {
            return Model.GetInheritedType()?.MenuOptions.CreationOptions.Any(x => x.Type.Id == creationOption.Type.Id) ??
                   false;
        }
    }
}