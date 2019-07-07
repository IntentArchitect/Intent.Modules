using System.Collections.Generic;
using Intent.Modules.Common.VisualStudio;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes
{
    internal class NuGetPackage
    {
        private NuGetPackage()
        {
        }

        public static NuGetPackage Create(INugetPackageInfo nugetPackageInfo, SemanticVersion version = null)
        {
            return new NuGetPackage
            {
                Version = version ?? SemanticVersion.Parse(nugetPackageInfo.Version),
                IncludeAssets = new List<string>(nugetPackageInfo.IncludeAssets ?? new string[0]),
                PrivateAssets = new List<string>(nugetPackageInfo.PrivateAssets ?? new string[0])
            };
        }

        public static NuGetPackage Create(string version, IEnumerable<string> includeAssets, IEnumerable<string> privateAssets)
        {
            return new NuGetPackage
            {
                Version = SemanticVersion.Parse(version),
                IncludeAssets = new List<string>(includeAssets),
                PrivateAssets = new List<string>(privateAssets)
            };
        }

        public void Update(SemanticVersion highestVersion, INugetPackageInfo nugetPackageInfo)
        {
            if (Version < highestVersion) Version = highestVersion;

            if (nugetPackageInfo.IncludeAssets != null)
            {
                foreach (var item in nugetPackageInfo.IncludeAssets)
                {
                    if (!IncludeAssets.Contains(item)) IncludeAssets.Add(item);
                }
            }

            if (nugetPackageInfo.PrivateAssets != null)
            {
                foreach (var item in nugetPackageInfo.PrivateAssets)
                {
                    if (!PrivateAssets.Contains(item)) PrivateAssets.Add(item);
                }
            }
        }

        public NuGetPackage Clone(SemanticVersion version = null)
        {
            return new NuGetPackage
            {
                Version = version ?? Version,
                PrivateAssets = new List<string>(PrivateAssets),
                IncludeAssets = new List<string>(IncludeAssets)
            };
        }

        public SemanticVersion Version { get; set; }

        public List<string> PrivateAssets { get; private set; }

        public List<string> IncludeAssets { get; private set; }
    }
}