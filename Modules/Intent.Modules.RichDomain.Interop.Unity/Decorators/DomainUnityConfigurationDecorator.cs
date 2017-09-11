using System.Collections.Generic;
using Intent.Packages.Unity;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.RichDomain.Interop.Unity.Decorators
{
    public class DomainUnityConfigurationDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Packages.RichDomain.Interop.Unity.Decorator";
        public IEnumerable<string> DeclareUsings()
        {
            return new []
            {
                "using Intent.Framework.Unity;"
            };
        }

        public string Registrations()
        {
            return @"    
            container.SetupDomainPublishing();
";
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkUnity
            };
        }
    }
}
