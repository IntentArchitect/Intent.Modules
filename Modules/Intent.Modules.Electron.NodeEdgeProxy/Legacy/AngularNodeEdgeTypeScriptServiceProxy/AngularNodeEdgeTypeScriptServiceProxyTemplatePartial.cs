using Intent.Modules.Electron.NodeEdgeProxy.Legacy.NodeEdgeCsharpReceivingProxy;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Electron.NodeEdgeProxy.Legacy.AngularNodeEdgeTypeScriptServiceProxy
{
    partial class AngularNodeEdgeTypeScriptServiceProxyTemplate : IntentProjectItemTemplateBase<ServiceModel>, ITemplate, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.AngularNodeEdgeTypeScriptServiceProxy.Legacy";
        private readonly IApplicationEventDispatcher _applicationEvents;

        public AngularNodeEdgeTypeScriptServiceProxyTemplate(ServiceModel model, IProject project, IApplicationEventDispatcher applicationEvents)
            : base(Identifier, project, model)
        {
            _applicationEvents = applicationEvents;
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
            //_applicationEvents.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            //{
            //    {"Key", ApiBasePathConfigKey},
            //    {"Value", Project.Application.HostingConfig.GetBaseUrl()},
            //});
        }
    }
}
