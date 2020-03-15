using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.metadata.StereotypeDefinitionFileTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class StereotypeDefinitionFileTemplate : IntentProjectItemTemplateBase<IStereotypeDefinition>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.metadata.StereotypeDefinitionFileTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore)]
        public StereotypeDefinitionFileTemplate(IProject project, IStereotypeDefinition model) : base(TemplateId, project, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "xml",
                defaultLocationInProject: $"metadata/{Model.GetParentPath().Single(x => x.SpecializationType == ModulePackage.SpecializationType).Name}/Stereotypes"
            );
        }

    }
}