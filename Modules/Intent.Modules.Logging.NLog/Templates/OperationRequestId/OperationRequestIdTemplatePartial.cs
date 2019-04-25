using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.Logging.NLog.Templates.OperationRequestId
{
    partial class OperationRequestIdTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Logging.NLog.OperationRequestId";

        public OperationRequestIdTemplate(IProject project)
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
                fileName: "OperationRequestId",
                fileExtension: "cs",
                defaultLocationInProject: "Context",
                className : "OperationRequestId",
                @namespace : "${Project.ProjectName}.Context"
                );
        }
    }
}
