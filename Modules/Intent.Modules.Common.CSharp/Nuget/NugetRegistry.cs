using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.CSharp.Nuget
{
    public class NugetRegistry
    {
        private static readonly Dictionary<string, NugetPackageRegistrations> _registrations = new();

        public static void Register(string packageName, Func<MajorMinorVersion, PackageVersion> versionInfo)
        {
            if (!_registrations.TryGetValue(packageName, out var registration))
            {
                registration = new NugetPackageRegistrations(packageName);
                _registrations.Add(packageName, registration);
            }
            registration.RegisteredVersions.Add(versionInfo);
        }

        public static NugetPackageInfo GetVersion(string packageName, MajorMinorVersion framework)
        {
            if (!_registrations.TryGetValue(packageName, out var registration))
            {
                throw new Exception($"NuGet package not registered : `{packageName}`");

            }
            var matchedVersions = registration.RegisteredVersions
                    .Select(versionFunc => versionFunc(framework))
                    .ToList();

            if (matchedVersions.Count == 0)
            {
                throw new Exception($"NuGet package : `{packageName}` has no supported versions for Framework ({framework.Major})");
            }

            var lockedVersions = matchedVersions.Where(v => v.Locked).ToList();
            if (lockedVersions.Any())
            {
                var lowestLockedVersion = lockedVersions.Min(v => v.Version);

                return GetNugetPackageInfo(packageName, lowestLockedVersion, lockedVersions.Where(x => x.Version == lowestLockedVersion));
            }
            else
            {
                var highestVersion = matchedVersions.Max(v => v.Version);
                return GetNugetPackageInfo(packageName, highestVersion, matchedVersions.Where(x => x.Version == highestVersion));
            }
        }

        private static NugetPackageInfo GetNugetPackageInfo(string packageName, NuGetVersion version, IEnumerable<PackageVersion> versionInfos)
        {
            var result =  new NugetPackageInfo(packageName, version.ToString());
            var includeAssets = versionInfos.SelectMany(x => x.IncludeAssets).Distinct();
            var privateAssets = versionInfos.SelectMany(x => x.PrivateAssets).Distinct();
            if (includeAssets.Any() || privateAssets.Any())
            {
                result.SpecifyAssetsBehaviour(privateAssets, includeAssets);
            }

            var consolidatedDependencies = versionInfos.SelectMany(x => x.Dependencies)
                .GroupBy(d => d.Name)
                .Select(g => g.First()) // Version should be identical as they are dependents for the same NuGet Package
                .ToList();
            if (consolidatedDependencies.Any())
            {
                foreach (var dependency in consolidatedDependencies)
                {
                    result.WithNugetDependency(dependency.Name, dependency.Version);
                }
            }
            return result;
        }

        private class NugetPackageRegistrations
        {
            public NugetPackageRegistrations(string packageName)
            {
                PackageName = packageName;
                RegisteredVersions = new List<Func<MajorMinorVersion, PackageVersion>>();
            }

            public string PackageName { get; }
            public List<Func<MajorMinorVersion, PackageVersion>> RegisteredVersions { get; }
        }
    }
}
