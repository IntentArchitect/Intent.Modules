using Intent.SoftwareFactory.Engine;
using Intent.Templates
using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface
{
    partial class HttpClientServiceInterfaceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasAssemblyDependencies
    {
        public const string IDENTIFIER = "Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface";

        public HttpClientServiceInterfaceTemplate(IProject project, string identifier = IDENTIFIER)
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
                fileName: "IHttpClientService",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated",
                className: "IHttpClientService",
                @namespace: "${Project.Name}");
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
