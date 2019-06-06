using System.Linq;
using Intent.MetaModel.Service;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Electron.IpcProxy.Templates.CSharpReceivingProxy;
using Intent.Modules.Typescript.ServiceAgent.Contracts;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.AngularTypeScriptIpcServiceProxy
{
    partial class AngularTypeScriptIpcServiceProxyTemplate : IntentProjectItemTemplateBase<IServiceModel>, ITemplate
    {
        public const string Identifier = "Intent.Electron.IpcProxy.AngularTypeScriptIpcServiceProxy";

        public AngularTypeScriptIpcServiceProxyTemplate(IServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
            var receivingProxyProject = project.Application.FindProjectWithTemplateInstance(CSharpIpcReceivingProxyTemplate.Identifier, model);
            AssemblyName = receivingProxyProject.Name;
        }

        public string Namespace => "App.Proxies";
        public string AssemblyName { get; }
        public string TypeName => $"{AssemblyName}.{Model.Name}NodeIpcProxy";

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: "wwwroot/App/Proxies/Generated");
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
