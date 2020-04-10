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
    [IntentManaged(Mode.Merge)]
    partial class ApiModelImplementationTemplate : IntentRoslynProjectItemTemplateBase<ElementSettings>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiModelImplementationTemplate";

        public ApiModelImplementationTemplate(IProject project, ElementSettings model) : base(TemplateId, project, model)
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
                @namespace: Model.Modeler.ApiNamespace
            );
        }

        public string BaseType => (!Model.GetSettings().TypeReference().IsDisabled()
                                   && Model.GetSettings().TargetTypes().Length == 1)
            ? GetTemplateClassName(TemplateId, Model.GetSettings().TargetTypes().Single().Id, throwIfNotFound: false) ?? Model.GetSettings().TargetTypes().Single().Name.ToCSharpIdentifier()
            : null;


        private string GetCreationOptionTypeInterface(CreationOption option, bool asCollection)
        {
            var @interface = GetTemplateClassName(ApiModelImplementationTemplate.TemplateId, option.Type.Id, throwIfNotFound: false);
            if (@interface == null)
            {
                return null;
            }
            return asCollection ? $"IList<{@interface}>" : @interface;
        }

        private string GetCreationOptionTypeClass(CreationOption option)
        {
            var className = GetTemplateClassName(TemplateId, option.Type.Id, throwIfNotFound: false);
            return className;
        }

        private string GetCreationOptionName(CreationOption option)
        {
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.AllowMultiple ? name.ToPluralName() : name;
        }
    }
}