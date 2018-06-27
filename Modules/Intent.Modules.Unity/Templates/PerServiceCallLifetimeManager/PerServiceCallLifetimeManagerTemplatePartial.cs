using System.Collections.Generic;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager
{
    partial class PerServiceCallLifetimeManagerTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Unity.PerServiceCallLifetimeManager";

        public PerServiceCallLifetimeManagerTemplate(IProject project)
            : base (Identifier, project, null)
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
                fileName: "PerServiceCallLifetimeManager",
                fileExtension: "cs",
                defaultLocationInProject: "Unity",
                className: "PerServiceCallLifetimeManager",
                @namespace: "${Project.Name}"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkCore
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
