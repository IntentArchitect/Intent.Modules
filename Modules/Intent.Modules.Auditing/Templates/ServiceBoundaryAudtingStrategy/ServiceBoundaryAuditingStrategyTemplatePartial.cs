using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.Auditing.Templates.ServiceBoundaryAudtingStrategy
{
    partial class ServiceBoundaryAuditingStrategyTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Auditing.ServiceBoundaryAuditingStrategy";
        public ServiceBoundaryAuditingStrategyTemplate(IProject project)
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
                fileName: $"ServiceBoundaryAuditingStrategy",
                fileExtension: "cs",
                defaultLocationInProject: "Auditing");
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkEntityFramework,
            };
        }
    }
}
