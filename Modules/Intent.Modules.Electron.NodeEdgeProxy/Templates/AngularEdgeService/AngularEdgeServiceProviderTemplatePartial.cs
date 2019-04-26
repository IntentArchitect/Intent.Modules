using System.Collections.Generic;
using Intent.MetaModel.Hosting;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Plugins;
using Intent.Templates;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularEdgeService
{
    partial class AngularEdgeServiceProviderTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.AngularEdgeServiceProvider";
        
        private readonly HostingConfigModel _hostingConfig;
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public AngularEdgeServiceProviderTemplate(IProject project, HostingConfigModel hostingConfig, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, null)
        {
            _hostingConfig = hostingConfig;
            _eventDispatcher = eventDispatcher;
        }

        public string ApiBasePathConfigKey => $"{Project.Application.SolutionName}_{Project.ApplicationName()}_api_basepath".ToLower();

        public void BeforeTemplateExecution()
        {
            _eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                { "Key", ApiBasePathConfigKey },
                { "Value", _hostingConfig?.GetBaseUrl() ?? "" }
            });
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "EdgeServiceProvider",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Services\Edge");
        }
    }
}
