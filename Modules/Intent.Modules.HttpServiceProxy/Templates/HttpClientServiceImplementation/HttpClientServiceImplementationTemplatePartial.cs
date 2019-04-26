using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceImplementation
{
    partial class HttpClientServiceImplementationTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasAssemblyDependencies, IPostTemplateCreation
    {
        public const string IDENTIFIER = "Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceImplementation";
        public const string HTTP_CLIENT_INTERCEPTOR_INTERFACE_TEMPLATE_ID_CONFIG_KEY = "HttpClientInterceptorInterfaceTemplateId";
        public const string HTTP_CLIENT_SERVICE_INTERFACE_TEMPLATE_ID_CONFIG_KEY = "HttpClientServiceInterfaceTemplateId";

        private string _httpClientInterceptorInterfaceTemplateId;
        private string _httpClientServiceInterfaceTemplateId;

        public HttpClientServiceImplementationTemplate(IProject project, string identifier = IDENTIFIER)
            : base (identifier, project, null)
        {
        }

        public void Created()
        {
            _httpClientInterceptorInterfaceTemplateId = GetMetaData().CustomMetaData[HTTP_CLIENT_INTERCEPTOR_INTERFACE_TEMPLATE_ID_CONFIG_KEY];
            _httpClientServiceInterfaceTemplateId = GetMetaData().CustomMetaData[HTTP_CLIENT_SERVICE_INTERFACE_TEMPLATE_ID_CONFIG_KEY];
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
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(_httpClientInterceptorInterfaceTemplateId);
            if (template == null)
            {
                throw new System.Exception($"Could not find template with ID [{_httpClientInterceptorInterfaceTemplateId}] " +
                                           $"as configured for the [{HTTP_CLIENT_INTERCEPTOR_INTERFACE_TEMPLATE_ID_CONFIG_KEY}] " +
                                           $"setting on the [{Id}] template.");
            }

            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }

        private string GetHttpClientServiceInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(_httpClientServiceInterfaceTemplateId);
            if (template == null)
            {
                throw new System.Exception($"Could not find template with ID [{_httpClientServiceInterfaceTemplateId}] " +
                                           $"as configured for the [{HTTP_CLIENT_SERVICE_INTERFACE_TEMPLATE_ID_CONFIG_KEY}] " +
                                           $"setting on the [{Id}] template.");
            }

            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }
    }
}
