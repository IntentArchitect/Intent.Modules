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
                var lowestLocked = lockedVersions.OrderBy(v => v.Version).First();
                return new NugetPackageInfo(packageName, lowestLocked.Version.ToString());
            }
            else
            {
                var highestUnlocked = matchedVersions.OrderByDescending(v => v.Version).First();
                return new NugetPackageInfo(packageName, highestUnlocked.Version.ToString());
            }
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

    public class PackageVersion
    {

        public bool Locked { get; }
        public NuGetVersion Version { get; }
        public PackageVersion(string version, bool locked = false)
        {
            Version = new NuGetVersion(version);
            Locked = locked;
        }
    }
}
