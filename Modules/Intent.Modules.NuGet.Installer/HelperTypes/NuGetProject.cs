using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Intent.SoftwareFactory.Engine;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.HelperTypes
{
    internal class NuGetProject
    {
        public string ProjectName { get; set; }
        public string ProjectTypeName { get; set; }
        public ProjectType Type { get; set; }
        public XDocument Document { get; set; }
        public Dictionary<string, SemanticVersion> RequestedPackages { get; set; } = new Dictionary<string, SemanticVersion>();
        public Dictionary<string, SemanticVersion> InstalledPackages { get; set; } = new Dictionary<string, SemanticVersion>();
        public string ProjectFile { get; set; }

        public Dictionary<string, SemanticVersion> GetConsolidatedPackages()
        {
            var allPackages = RequestedPackages.ToList();
            allPackages.AddRange(InstalledPackages);

            return allPackages
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.Max());
        }
    }
}