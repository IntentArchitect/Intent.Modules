using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HttpServiceProxy.Templates.InterceptorInterface
{
    partial class HttpProxyInterceptorInterfaceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasAssemblyDependencies
    {
        public const string IDENTIFIER = "Intent.HttpServiceProxy.InterceptorInterface";

        public HttpProxyInterceptorInterfaceTemplate(IProject project, string identifier = IDENTIFIER)
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
                fileName: "IHttpProxyInterceptor",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated",
                className: "IHttpProxyInterceptor",
                @namespace: "${Project.Name}"
                );
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http"),
            };
        }
    }
}
