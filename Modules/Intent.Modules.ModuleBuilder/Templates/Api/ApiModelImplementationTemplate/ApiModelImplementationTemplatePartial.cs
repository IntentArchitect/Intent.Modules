using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelImplementationTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class ApiModelImplementationTemplate : IntentRoslynProjectItemTemplateBase<IElementSettings>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiModelImplementationTemplate";

        public ApiModelImplementationTemplate(IProject project, IElementSettings model) : base(TemplateId, project, model)
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
                fileName: Model.Name.ToCSharpIdentifier(),
                fileExtension: "cs",
                defaultLocationInProject: "Api/Implementation",
                className: Model.Name.ToCSharpIdentifier(),
                @namespace: "${Project.Name}.Api"
            );
        }

        public string InterfaceName => GetTemplateClassName(ApiModelInterfaceTemplate.ApiModelInterfaceTemplate.TemplateId, Model);

        private string GetCreationOptionType(ICreationOption option)
        {
            var @interface = GetTemplateClassName(ApiModelInterfaceTemplate.ApiModelInterfaceTemplate.TemplateId, option.Type.Id, throwIfNotFound: false);
            return option.AllowMultiple ? $"IList<{@interface}>" : @interface;
        }

        private string GetCreationOptionName(ICreationOption option)
        {
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.AllowMultiple ? name.ToPluralName() : name;
        }
    }
}