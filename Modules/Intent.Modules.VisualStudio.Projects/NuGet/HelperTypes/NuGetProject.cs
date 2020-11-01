using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes
{
    internal class NuGetProject
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public Dictionary<string, NuGetPackage> RequestedPackages { get; set; } = new Dictionary<string, NuGetPackage>();
        public Dictionary<string, NuGetPackage> InstalledPackages { get; set; } = new Dictionary<string, NuGetPackage>();
        public Dictionary<string, VersionRange> HighestVersions { get; set; } = new Dictionary<string, VersionRange>();
        public string FilePath { get; set; }
        public INuGetSchemeProcessor Processor { get; set; }

        public Dictionary<string, NuGetPackage> GetConsolidatedPackages()
        {
            var allPackages = RequestedPackages.ToList();
            allPackages.AddRange(InstalledPackages);

            return allPackages
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.Version.MinVersion).First());
        }
    }
}