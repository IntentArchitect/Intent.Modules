using Intent.Modules.Logging.NLog.Templates.OperationRequestId;
using Intent.Engine;
using Intent.Templates
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Logging.NLog.Templates.OperationRequestIdRenderer
{
    partial class NLogOperationRequestIdRendererTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Logging.NLog.OperationRequestIdRenderer";

        public NLogOperationRequestIdRendererTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NLog,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(OperationRequestIdTemplate.Identifier)
            };            
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "NLogOperationRequestIdRenderer",
                fileExtension: "cs",
                defaultLocationInProject: "Logging",
                className: "NLogOperationRequestIdRenderer",
                @namespace: "${Project.Name}.Logging"
                );
        }
    }
}
