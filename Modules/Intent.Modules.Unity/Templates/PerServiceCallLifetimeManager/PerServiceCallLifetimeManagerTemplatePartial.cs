using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.Templates;

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
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "PerServiceCallLifetimeManager",
                fileExtension: "cs",
                defaultLocationInProject: "Unity",
                className: "PerServiceCallLifetimeManager",
                @namespace: "${Project.Name}.Unity"
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
