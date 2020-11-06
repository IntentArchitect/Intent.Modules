using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.HttpServiceProxy.Templates.Exception
{
    partial class WebApiClientServiceProxyRemoteExceptionTemplate : CSharpTemplateBase<object>, ITemplate
    {
        public const string IDENTIFIER = "Intent.HttpServiceProxy.Exception";

        public WebApiClientServiceProxyRemoteExceptionTemplate(IProject project, string identifier = IDENTIFIER)
            : base (identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "WebApiClientServiceProxyRemoteException",
                fileExtension: "cs",
                relativeLocation: @"Generated",
                className: "WebApiClientServiceProxyRemoteException",
                @namespace: "${Project.Name}"
                );
        }
    }
}
