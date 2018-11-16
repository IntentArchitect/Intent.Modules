using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

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
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
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
