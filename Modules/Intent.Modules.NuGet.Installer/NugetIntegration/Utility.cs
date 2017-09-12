using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace Intent.SoftwareFactory.NuGet.NugetIntegration
{
    public static class Utility
    {
        public static IVersionSpec ParseVersionSpec(string value) => VersionUtility.ParseVersionSpec(value);

        public static bool IsSatisfiedBy(this IVersionSpec versionSpec, SemanticVersion version)
        {
            return versionSpec.Satisfies(version);
        }

        public static IVersionSpec GetEmptyVersionSpec()
        {
            return new VersionSpec();
        }

        public static IVersionSpec GetMinVersionSpecInclusive(string minVersionInclusive)
        {
            return SemanticVersion.Parse(minVersionInclusive).ToMinVersionSpecInclusive();
        }

        public static bool RequiresLowerThan(this IVersionSpec versionSpec, SemanticVersion version)
        {
            if (versionSpec.MaxVersion == null)
            {
                return false;
            }

            return versionSpec.IsMaxInclusive
                ? version > versionSpec.MaxVersion
                : version >= versionSpec.MaxVersion;
        }

        public static ICollection<PackageDependency> GetDependencies(this IPackage package, string targetFrameworkName)
        {
            var targetFramework = new FrameworkName(targetFrameworkName);

            if (!package.DependencySets.Any())
            {
                return new List<PackageDependency>();
            }

            var dependencySet = package.DependencySets.Where(x =>
                        x.TargetFramework == null ||
                        (x.TargetFramework.Version <= targetFramework.Version &&
                        x.TargetFramework.Identifier.Equals(targetFramework.Identifier, StringComparison.InvariantCultureIgnoreCase))).ToList();

            if (!dependencySet.Any())
            {
                return new PackageDependency[] { };
            }

            if (dependencySet.Count() == 1)
            {
                return dependencySet.Single().Dependencies;
            }

            return dependencySet.OrderByDescending(x => x.TargetFramework?.Version).First().Dependencies;
        }

        public static IVersionSpec ToMinVersionSpecInclusive(this SemanticVersion minVersionInclusive)
        {
            return new VersionSpec
            {
                IsMinInclusive = true,
                MinVersion = minVersionInclusive
            };
        }

        public static IVersionSpec ToCombinedMinVersionSpecInclusive(this IVersionSpec versionSpec, SemanticVersion minVersionInclusive)
        {
            if (versionSpec == null)
            {
                return minVersionInclusive.ToMinVersionSpecInclusive();
            }

            if (!versionSpec.IsSatisfiedBy(minVersionInclusive))
            {
                return new VersionSpec
                {
                    MinVersion = versionSpec.MinVersion,
                    IsMinInclusive = versionSpec.IsMinInclusive,
                    MaxVersion = versionSpec.MaxVersion,
                    IsMaxInclusive = versionSpec.IsMaxInclusive,
                };

            }

            return new VersionSpec
            {
                MinVersion = minVersionInclusive,
                IsMinInclusive = true,
                MaxVersion = versionSpec.MaxVersion,
                IsMaxInclusive = versionSpec.IsMaxInclusive,
            };
        }
    }
}
