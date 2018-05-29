using System.Collections.Generic;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Entities.Interop.EntityFramework.Templates.IdentityGenerator
{
    partial class IdentityGeneratorTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Entities.Interop.EntityFramework.IdentityGenerator";

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
                new NugetPackageInfo("RT.Comb", "2.3.0", null)
            };
        }
    }
}
