using Intent.MetaModel.Hosting;
using Intent.MetaModel.Service;
using Intent.Modules.Constants;
using Intent.Modules.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.Linq;
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

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\Proxies\Generated");
        }

        private string GetReturnType(IOperationModel operation)
        {
            return operation.ReturnType != null
                ? this.ConvertType(operation.ReturnType.TypeReference)
                : "void";
        }

        private static string GetMethodCallParameters(IOperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return ", " + operation.Parameters
                .Select(x => x.Name.ToCamelCase())
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodDefinitionParameters(IOperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {this.ConvertType(x.TypeReference)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }
    }
}
