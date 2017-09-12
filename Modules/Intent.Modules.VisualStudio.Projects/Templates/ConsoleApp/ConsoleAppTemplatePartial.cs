using System.Collections.Generic;
using System.Linq;
using Intent.SoftwareFactory.Engine;

using Intent.SoftwareFactory.RoslynMerge;
using Intent.SoftwareFactory.RoslynMerge.Migrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Configuration;

namespace Intent.SoftwareFactory.VSProjects.Templates.ConsoleApp
{
    partial class ConsoleAppTemplate : IntentRoslynProjectItemTemplateBase<object>
    {
        public ConsoleAppTemplate(IProject project)
            : base(CoreTemplateId.ConsoleApp, project, null)
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
