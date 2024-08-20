using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NugetRegistryLoaderExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.CSharp.NugetRegistryLoaderExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {

            var nugetRegistrations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(c => c.GetExportedTypes())
                .Where(t => typeof(INugetPackages).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();

            foreach (var nugetPackageRegistration in nugetRegistrations)
            {
                INugetPackages current = Activator.CreateInstance(nugetPackageRegistration) as INugetPackages;
                current.RegisterPackages();
            }
        }
    }
}