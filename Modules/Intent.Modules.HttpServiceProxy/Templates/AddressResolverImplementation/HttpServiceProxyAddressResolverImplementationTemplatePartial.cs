using Intent.Modules.HttpServiceProxy.Templates.AddressResolverInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.HttpServiceProxy.Templates.AddressResolverImplementation
{
    partial class HttpServiceProxyAddressResolverImplementationTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.HttpServiceProxy.AddressResolverImplementation";

        public HttpServiceProxyAddressResolverImplementationTemplate(IProject project)
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
                fileName: "HttpServiceProxyAddressResolver",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated",
                className: "HttpServiceProxyAddressResolver",
                @namespace: "${Project.Name}");
        }

        private string GetAddressResolverInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(HttpServiceProxyAddressResolverInterfaceTemplate.Identifier));
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }
    }
}
