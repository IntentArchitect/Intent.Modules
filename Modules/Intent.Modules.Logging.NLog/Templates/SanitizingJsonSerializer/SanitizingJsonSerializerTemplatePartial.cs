using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Logging.NLog.Templates.SanitizingJsonSerializer
{
    partial class SanitizingJsonSerializerTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasAssemblyDependencies
    {
        public const string Identifier = "Intent.Logging.NLog.SanitizingJsonSerializer";

        public SanitizingJsonSerializerTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http"),
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NewtonsoftJson,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "SanitizingJsonSerializer",
                fileExtension: "cs",
                defaultLocationInProject: "Logging",
                className: "SanitizingJsonSerializer",
                @namespace: "${Project.Name}.Logging"
                );
        }
    }
}
