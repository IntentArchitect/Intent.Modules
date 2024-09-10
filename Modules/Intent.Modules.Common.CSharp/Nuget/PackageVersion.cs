using Intent.Modules.Common.VisualStudio;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Nuget
{
    public class PackageVersion
    {
        private readonly List<INugetPackageDependency> _dependencies;

        public bool Locked { get; }
        public NuGetVersion Version { get; }
        public string[] PrivateAssets { get; private set; }
        public string[] IncludeAssets { get; private set; }
        public IReadOnlyList<INugetPackageDependency> Dependencies { get => _dependencies; }

        public PackageVersion(string version, bool locked = false)
        {
            Version = new NuGetVersion(version);
            Locked = locked;
            PrivateAssets = new string[0];
            IncludeAssets = new string[0];
            _dependencies = new List<INugetPackageDependency>();
        }

        public PackageVersion WithNugetDependency(string packageName, string version)
        {
            _dependencies.Add(new NugetPackageDependency(packageName, version));
            return this;
        }
        public PackageVersion SpecifyAssetsBehaviour(IEnumerable<string> privateAssets = null, IEnumerable<string> includeAssets = null)
        {

            PrivateAssets = privateAssets as string[] ?? privateAssets?.ToArray();
            IncludeAssets = includeAssets as string[] ?? includeAssets?.ToArray();

            return this;
        }
    }
}

