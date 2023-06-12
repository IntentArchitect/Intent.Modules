using System;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NugetVersionSetterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModuleBuilder.NugetVersionSetterExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var depVerMan = application.MetadataManager
                .ModuleBuilder(application)
                ?.GetIntentModuleModels()
                ?.FirstOrDefault()
                ?.GetModuleSettings()
                ?.DependencyVersionManagement();

            var option = depVerMan?.Value == null
                ? IntentModuleModelStereotypeExtensions.ModuleSettings.DependencyVersionManagementOptionsEnum.OnlyIfNewer
                : depVerMan.AsEnum();

            foreach (var outputTarget in application.OutputTargets)
            {
                outputTarget.SetDependencyVersionManagement(option switch
                {
                    IntentModuleModelStereotypeExtensions.ModuleSettings.DependencyVersionManagementOptionsEnum.OnlyIfMissing
                        => DependencyVersionManagement.OnlyIfMissing,
                    IntentModuleModelStereotypeExtensions.ModuleSettings.DependencyVersionManagementOptionsEnum.OnlyIfNewer
                        => DependencyVersionManagement.OnlyIfNewer,
                    IntentModuleModelStereotypeExtensions.ModuleSettings.DependencyVersionManagementOptionsEnum.AlwaysOverwrite
                        => DependencyVersionManagement.AlwaysOverwrite,
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
        }
    }
}