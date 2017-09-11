using System.Collections.Generic;
using System.Linq;
using Intent.Package.Logging.NLog;
using Intent.Packages.Logging.NLog.Templates.OperationRequestId;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Logging.NLog.Templates.OperationRequestIdRenderer
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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(OperationRequestIdTemplate.Identifier)
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
