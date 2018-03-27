using Intent.MetaModel.Hosting;
using Intent.MetaModel.Service;
using Intent.Modules.Constants;
using Intent.Modules.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Typescript.ServiceAgent.Contracts;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularNodeEdgeTypeScriptServiceProxy
{
    partial class AngularNodeEdgeTypeScriptServiceProxyTemplate : IntentProjectItemTemplateBase<ServiceModel>, ITemplate, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.AngularNodeEdgeTypeScriptServiceProxy";

        private readonly HostingConfigModel _hostingConfig;
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public AngularNodeEdgeTypeScriptServiceProxyTemplate(ServiceModel model, HostingConfigModel hostingConfig, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, model)
        {
            _hostingConfig = hostingConfig;
            _eventDispatcher = eventDispatcher;
            var recevingProxyProject = project.Application.FindProjectWithTemplateInstance(NodeEdgeCsharpReceivingProxyTemplate.Identifier, model);
            AssemblyName = recevingProxyProject.Name;
        }

        public string Namespace => "App.Proxies";
        public string SolutionName => Project.Application.SolutionName;
        public string ApplicationName => Project.Application.ApplicationName;
        public string ApiBasePathConfigKey => $"{Project.Application.SolutionName}_{Project.ApplicationName()}_api_basepath".ToLower();
        public string AssemblyName { get; }
        public string TypeName => $"{AssemblyName}.{Model.Name}NodeEdgeProxy";

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\Proxies\Generated"
                );
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                { "Key", ApiBasePathConfigKey },
                { "Value", _hostingConfig?.GetBaseUrl() ?? "" }
            });
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
