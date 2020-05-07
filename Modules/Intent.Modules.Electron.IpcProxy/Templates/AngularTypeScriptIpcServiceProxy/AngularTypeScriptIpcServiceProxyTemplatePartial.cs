using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Electron.IpcProxy.Templates.CSharpReceivingProxy;
using Intent.Modules.Typescript.ServiceAgent.Contracts;
using Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO;
using Intent.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.AngularTypeScriptIpcServiceProxy
{
    partial class AngularTypeScriptIpcServiceProxyTemplate : IntentProjectItemTemplateBase<ServiceModel>, ITemplate
    {
        public const string Identifier = "Intent.Electron.IpcProxy.AngularTypeScriptIpcServiceProxy";

        public AngularTypeScriptIpcServiceProxyTemplate(ServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
            AddTypeSource(TypescriptTypeSource.InProject(Project, TypescriptDtoTemplate.LocalIdentifier));
            AddTypeSource(TypescriptTypeSource.InProject(Project, TypescriptDtoTemplate.RemoteIdentifier));
            var receivingProxyProject = project.Application.FindProjectWithTemplateInstance(CSharpIpcReceivingProxyTemplate.Identifier, model);
            AssemblyName = receivingProxyProject.Name;
        }

        public string Namespace => "App.Proxies";
        public string AssemblyName { get; }
        public string TypeName => $"{AssemblyName}.{Model.Name}NodeIpcProxy";

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: "wwwroot/App/Proxies/Generated");
        }

        private string GetReturnType(OperationModel operation)
        {
            return operation.ReturnType != null
                ? GetTypeName(operation.ReturnType)
                : "void";
        }

        private static string GetMethodCallParameters(OperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return ", " + operation.Parameters
                .Select(x => x.Name.ToCamelCase())
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodDefinitionParameters(OperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {GetTypeName(x.Type)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }
    }
}
