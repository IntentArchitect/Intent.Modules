namespace Intent.Modules.NuGet.Installer.Managers
{
    public class NuGetManagerSettings
    {
        public bool ConsolidateVersions { get; set; }
        public bool AllowPreReleaseVersions { get; set; }
        public bool WarnOnMultipleVersionsOfSamePackage { get; set; }

    }
}