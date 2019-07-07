using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.NuGet.Installer
{
    public class NugetInstallerFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override int Order => 200;

        public override string Id => "Intent.NugetInstaller";

        public void OnStep(IApplication application, string step)
        {
            if (step != ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                return;
            }

            Logging.Log.Warning(
                "The `Intent.NuGet.Installer` module along with its `Intent.NugetInstaller` extension has been deprecated. You should " +
                "instead use the NuGet installer which is part of the `Intent.VisualStudio.Projects` module. You can uninstall the " +
                "`Intent.NuGet.Installer` module to stop seeing this warning.");
        }
    }
}
