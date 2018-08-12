using Intent.Modules.Unity;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.RichDomain.Interop.Unity.Decorators
{
    public class DomainUnityConfigurationDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Packages.RichDomain.Interop.Unity.Decorator";
        public IEnumerable<string> DeclareUsings()
        {
            return new []
            {
                "Intent.Framework.Unity"
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
                new NugetPackageInfo("Intent.Framework.Unity", "0.1.2-beta"), 
            };
        }
    }
}
