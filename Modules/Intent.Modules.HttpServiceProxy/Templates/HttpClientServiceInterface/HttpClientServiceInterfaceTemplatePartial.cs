using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface
{
    partial class HttpClientServiceInterfaceTemplate : CSharpTemplateBase<object>, ITemplate, IHasAssemblyDependencies
    {
        public const string IDENTIFIER = "Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface";

        public HttpClientServiceInterfaceTemplate(IProject project, string identifier = IDENTIFIER)
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
                fileName: "IHttpClientService",
                fileExtension: "cs",
                relativeLocation: @"Generated",
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
