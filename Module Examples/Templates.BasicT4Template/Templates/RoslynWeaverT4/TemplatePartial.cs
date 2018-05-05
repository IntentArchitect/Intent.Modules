using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.BasicT4Template.Templates.RoslynWeaverT4
{
    public partial class Template : Intent.SoftwareFactory.Templates.IntentRoslynProjectItemTemplateBase
    {
        public const string Identifier = "Templates.BasicT4Template.RoslynWeaverT4";

        public Template(IProject project)
            : base(Identifier, project)
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
                fileName: "RoslynWeaverT4",
                fileExtension: "cs",
                defaultLocationInProject: "Templates\\BasicT4Template\\RoslynWeaverT4",
                className: "RoslynWeaverT4",
                @namespace: Project.Name + ".Templates.BasicT4Template"
                );
        }

        internal string GetTypeFromPartial()
        {
            return "int";
        }
    }
}
