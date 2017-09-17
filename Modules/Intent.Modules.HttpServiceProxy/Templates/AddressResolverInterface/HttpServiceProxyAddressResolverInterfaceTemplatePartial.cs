using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.HttpServiceProxy.Templates.AddressResolverInterface
{
    partial class HttpServiceProxyAddressResolverInterfaceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.HttpServiceProxy.AddressResolverInterface";

        public HttpServiceProxyAddressResolverInterfaceTemplate(IProject project)
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
                fileName: "IHttpServiceProxyAddressResolver",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated",
                className: "IHttpServiceProxyAddressResolver",
                @namespace: "${Project.Name}");
        }
    }
}
