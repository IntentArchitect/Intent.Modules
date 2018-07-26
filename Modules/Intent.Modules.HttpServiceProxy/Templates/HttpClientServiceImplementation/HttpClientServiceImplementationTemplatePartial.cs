using System.Collections.Generic;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface;
using Intent.Modules.HttpServiceProxy.Templates.InterceptorInterface;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceImplementation
{
    partial class HttpClientServiceImplementationTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasAssemblyDependencies
    {
        public const string IDENTIFIER = "Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceImplementation";

        public HttpClientServiceImplementationTemplate(IProject project)
            : base (IDENTIFIER, project, null)
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
                fileName: "HttpClientService",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated",
                className: "HttpClientService",
                @namespace: "${Project.Name}");
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http"),
            };
        }

        private string GetInterceptorInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(HttpProxyInterceptorInterfaceTemplate.Identifier));
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }

        private string GetHttpClientServiceInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(HttpClientServiceInterfaceTemplate.IDENTIFIER));
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }
    }
}
