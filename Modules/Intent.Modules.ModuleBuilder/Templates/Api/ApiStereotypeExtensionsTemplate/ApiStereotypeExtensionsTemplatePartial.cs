using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiStereotypeExtensionsTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class ApiStereotypeExtensionsTemplate : IntentRoslynProjectItemTemplateBase<IStereotypeDefinition>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiStereotypeExtensionsTemplate";

        public ApiStereotypeExtensionsTemplate(IProject project, IStereotypeDefinition model) : base(TemplateId, project, model)
        {
            AddTypeSource(CSharpTypeSource.InProject(Project, ApiStereotypeExtensionsTemplate.TemplateId, collectionFormat: "IEnumerable<{0}>"));
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
                fileName: $"{Model.Name.ToCSharpIdentifier()}ApiExtension",
                fileExtension: "cs",
                defaultLocationInProject: "ApiStereotypeExtensions",
                className: $"{Model.Name.ToCSharpIdentifier()}ApiExtension",
                @namespace: "${Project.Name}.Api"
            );
        }

        public string InterfaceName => $"I{Model.Name.ToCSharpIdentifier()}";


        public IEnumerable<string> GetTargetInterfaces()
        {
            return Model.TargetElements
                .Select(x => GetTemplateClassName(TemplateDependency.OnModel<IElementSettings>(ApiModelInterfaceTemplate.ApiModelInterfaceTemplate.TemplateId, (model) => model.Name.Equals(x, StringComparison.InvariantCultureIgnoreCase)), throwIfNotFound: false))
                .Where(x => x != null);
        }
    }
}