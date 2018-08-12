using Intent.Modules.Logging.NLog.Templates.OperationRequestIdRenderer;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.Logging.NLog.Interop.Unity.Decorators
{
    public class NLogUnityDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Logging.NLog.Interop.Unity.Decorator";

        private readonly IApplication _applicationManager;

        public NLogUnityDecorator(IApplication applicationManager)
        {
            _applicationManager = applicationManager;
        }

        public string Registrations() => @"
            NLogOperationRequestIdRenderer.Register();";

        public IEnumerable<string> DeclareUsings() => new string[]
        {
            _applicationManager.FindTemplateInstance<IHasClassDetails>(NLogOperationRequestIdRendererTemplate.Identifier).Namespace ,
        };

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NLog,
            };
        }
    }
}