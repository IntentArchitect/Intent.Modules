using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Keys.Templates.IdentityGenerator
{
    partial class IdentityGeneratorTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Entities.Keys.IdentityGenerator";

        public IdentityGeneratorTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "IdentityGenerator",
                fileExtension: "cs",
                defaultLocationInProject: "Common",
                className: "IdentityGenerator",
                @namespace: "${Project.ProjectName}.Common"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>()
            {
                new NugetPackageInfo("RT.Comb", "2.3.0")
            }.Union(base.GetNugetDependencies());
        }
    }
}
