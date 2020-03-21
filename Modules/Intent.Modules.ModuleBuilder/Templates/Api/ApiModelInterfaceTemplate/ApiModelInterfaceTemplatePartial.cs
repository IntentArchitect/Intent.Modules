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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class ApiModelInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IElementSettings>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate";

        public ApiModelInterfaceTemplate(IProject project, IElementSettings model) : base(TemplateId, project, model)
        {
            AddTypeSource(CSharpTypeSource.InProject(Project, ApiModelInterfaceTemplate.TemplateId, collectionFormat: "IEnumerable<{0}>"));
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
                fileName: $"I{Model.Name.ToCSharpIdentifier()}",
                fileExtension: "cs",
                defaultLocationInProject: "Api",
                className: $"I{Model.Name.ToCSharpIdentifier()}",
                @namespace: "${Project.Name}.Api"
            );
        }

        public string BaseType => (!Model.GetSettings().TypeReference().IsDisabled()
                                   && Model.GetSettings().TargetTypes().Length == 1)
            ? GetTemplateClassName(TemplateId, Model.GetSettings().TargetTypes().Single().Id, throwIfNotFound: false) ?? $"I{Model.GetSettings().TargetTypes().Single().Name.ToCSharpIdentifier()}"
            : null;

        private string GetCreationOptionType(ICreationOption option)
        {
            var @interface = GetTemplateClassName(ApiModelInterfaceTemplate.TemplateId, option.Type.Id, throwIfNotFound: false);
            if (@interface == null)
            {
                return null;
            }
            return option.AllowMultiple ? $"IList<{@interface}>" : @interface;
        }

        private string GetCreationOptionName(ICreationOption option)
        {
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.AllowMultiple ? name.ToPluralName() : name;
        }

    }
}