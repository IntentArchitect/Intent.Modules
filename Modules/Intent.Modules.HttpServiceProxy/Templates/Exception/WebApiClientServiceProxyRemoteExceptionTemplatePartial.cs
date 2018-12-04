using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.HttpServiceProxy.Templates.Exception
{
    partial class WebApiClientServiceProxyRemoteExceptionTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string IDENTIFIER = "Intent.HttpServiceProxy.Exception";

        public WebApiClientServiceProxyRemoteExceptionTemplate(IProject project, string identifier = IDENTIFIER)
            : base (identifier, project, null)
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
                fileName: "WebApiClientServiceProxyRemoteException",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated",
                className: "WebApiClientServiceProxyRemoteException",
                @namespace: "${Project.Name}"
                );
        }
    }
}
