using System.Collections.Generic;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Auditing.Templates.HttpRequestMessageExtensions
{
    partial class HttpRequestMessageExtensionsTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Auditing.HttpRequestMessageExtensions";
        public HttpRequestMessageExtensionsTemplate(IProject project)
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
                fileName: $"HttpRequestMessageExtensions",
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
