using System;
using System.Linq;
using System.Reflection;
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
                .Where(assembly =>
                {
                    try
                    {
                        // Attempt to access the exported types.
                        var types = assembly.GetExportedTypes();
                        return types != null; // Only include assemblies that successfully return types.
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Assembly could not load all types.
                        return false;
                    }
                    catch (NotSupportedException)
                    {
                        // Assembly does not support getting exported types.
                        return false;
                    }
                    catch (Exception)
                    {
                        // Catch other exceptions just in case.
                        return false;
                    }
                })
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(t => typeof(INugetPackages).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();

            foreach (var nugetPackageRegistration in nugetRegistrations)
            {
                INugetPackages current;

                // TODO: Expose an `IServiceProvider` through the SoftwareFactory.SDK to make this
                // type resolution cleaner (not to mention allow for more injectable services).
                if (nugetPackageRegistration.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .Where(ctor => ctor.GetParameters().Length == 1)
                    .Any(ctor => ctor.GetParameters().First().ParameterType == typeof(IApplicationSettingsProvider)))
                {
                    current = Activator.CreateInstance(nugetPackageRegistration, application.Settings) as INugetPackages;
                }
                else
                {
                    current = Activator.CreateInstance(nugetPackageRegistration) as INugetPackages;
                }

                current.RegisterPackages();
            }
        }
    }
}