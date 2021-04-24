using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Templates.IatSpecFile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class IatSpecFileTemplate : IntentTemplateBase<ApplicationTemplateModel>, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ApplicationTemplate.Builder.Templates.IatSpecFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public IatSpecFileTemplate(IOutputTarget outputTarget, ApplicationTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public IIconModel Icon => Model.Icon ?? OutputTarget.Application.Icon;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"metadata",
                fileExtension: "iatspec",
                relativeLocation: Model.Name
            );
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] { IntentNugetPackages.IntentPackager };
        }
    }
}