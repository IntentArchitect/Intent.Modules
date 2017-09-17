using NuGet;

namespace Intent.Modules.NuGet.Installer.NugetIntegration
{
    public interface INugetServices
    {
        void RestorePackages(string solutionFilePath);
        IPackage GetPackage(string packageId, IVersionSpec versionSpec, bool allowPrereleaseVersions);
        bool IsInstalled(string project, IPackage package);
        void Install(string project, IPackage package);
        void Uninstall(string project, IPackage package);
        IPackage[] GetInstalled(string project);
        string GetTargetFrameworkName(string projectFile);
        void CleanupPackagesFolder();
    }
}