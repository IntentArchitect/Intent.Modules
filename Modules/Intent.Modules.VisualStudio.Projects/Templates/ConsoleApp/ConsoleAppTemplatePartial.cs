using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp
{
    partial class ConsoleAppTemplate : IntentRoslynProjectItemTemplateBase<object>
    {
#warning this wasnt being used but looks like maybe it should ???B

        public ConsoleAppTemplate(IProject project)
            : base("HELP ME PLZ", project, null)
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
                className:null,
                @namespace: null
                );
        }
    }
}
