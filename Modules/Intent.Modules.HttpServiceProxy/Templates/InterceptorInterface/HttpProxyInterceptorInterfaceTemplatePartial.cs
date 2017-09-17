using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.HttpServiceProxy.Templates.InterceptorInterface
{
    partial class HttpProxyInterceptorInterfaceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasAssemblyDependencies
    {
        public const string Identifier = "Intent.HttpServiceProxy.InterceptorInterface";

        public HttpProxyInterceptorInterfaceTemplate(IProject project)
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
