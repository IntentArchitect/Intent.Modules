using Intent.Modelers.Services.Api;
using Intent.Modules.Constants;
using Intent.Modules.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy;
using Intent.Engine;
using Intent.Eventing;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Typescript.ServiceAgent.Contracts;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularNodeEdgeTypeScriptServiceProxy
{
    partial class AngularNodeEdgeTypeScriptServiceProxyTemplate : IntentProjectItemTemplateBase<IServiceModel>, ITemplate
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.AngularNodeEdgeTypeScriptServiceProxy";

        public AngularNodeEdgeTypeScriptServiceProxyTemplate(IServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
            var receivingProxyProject = project.Application.FindProjectWithTemplateInstance(NodeEdgeCsharpReceivingProxyTemplate.Identifier, model);
            AssemblyName = receivingProxyProject.Name;
        }

        public string Namespace => "App.Proxies";
        public string AssemblyName { get; }
        public string TypeName => $"{AssemblyName}.{Model.Name}NodeEdgeProxy";

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\Proxies\Generated");
        }

        private string GetReturnType(IOperation operation)
        {
            return operation.ReturnType != null
                ? this.ConvertType(operation.ReturnType.Type)
                : "void";
        }

        private static string GetMethodCallParameters(IOperation operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return ", " + operation.Parameters
                .Select(x => x.Name.ToCamelCase())
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodDefinitionParameters(IOperation operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {this.ConvertType(x.Type)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }
    }
}
