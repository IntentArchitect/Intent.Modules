using System.Collections.Generic;
using Intent.Package.Logging.NLog;
using Intent.Packages.Logging.NLog.Templates.OperationRequestIdRenderer;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Logging.NLog.Interop.Unity.Decorators
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
            $"using {_applicationManager.FindTemplateInstance<IHasClassDetails>(NLogOperationRequestIdRendererTemplate.Identifier).Namespace };",
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