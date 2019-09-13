using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Templates.Program
{
    partial class CoreWebProgramTemplate : IntentRoslynProjectItemTemplateBase<object>
    {
        public const string Identifier = "Intent.AspNetCore.Program";

        public CoreWebProgramTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"Program",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: $"Program",
                @namespace: "${Project.Name}"
                );
        }
    }
}
