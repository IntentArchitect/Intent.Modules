using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intent.Utils;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Intent.Modules.ModuleBuilder.CSharp.Tasks
{
    internal class NuGetApi
    {

        private static List<NuGetFramework> _frameworks = new List<NuGetFramework>
            {
                NuGetFramework.Parse(".NETStandard,Version=v2.0"),
                NuGetFramework.Parse(".NETStandard,Version=v2.1"),
                NuGetFramework.Parse(".NETCoreApp,Version=v6.0"),
                NuGetFramework.Parse(".NETCoreApp,Version=v7.0"),
                NuGetFramework.Parse(".NETCoreApp,Version=v8.0"),
                NuGetFramework.Parse(".NETCoreApp,Version=v9.0")
            };

        private static List<NuGetFramework> _fallBackFrameworks = new List<NuGetFramework>
            {
                NuGetFramework.Parse("Any,Version=v0.0"),
            };


        public static async Task<List<NugetVersionInfo>> GetLatestVersionsForFrameworksAsync(string packageName)
        {


            Logging.Log.Info($"Executing: Fetching Package Details : {packageName}");
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());

            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);

            var packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var searchMetadata = await packageMetadataResource.GetMetadataAsync(packageName, includePrerelease: false, includeUnlisted: false, new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);


            var versionsForFrameworks = GetLatestVersionPerFramework(searchMetadata, _frameworks);

            if (!versionsForFrameworks.Any())
            {
                versionsForFrameworks = GetLatestVersionPerFramework(searchMetadata, _fallBackFrameworks);
            }

            return versionsForFrameworks;
        }

        private static List<NugetVersionInfo> GetLatestVersionPerFramework(IEnumerable<IPackageSearchMetadata>? searchMetadata, List<NuGetFramework> frameworks)
        {
            var result = new List<NugetVersionInfo>();
            foreach (var framework in frameworks)
            {
                var latestPackage = searchMetadata?
                    .Where(p => p.DependencySets.Any(d => d.TargetFramework == framework))
                    .OrderByDescending(p => p.Identity.Version)
                    .FirstOrDefault();

                if (latestPackage != null)
                {
                    var dependencies = latestPackage.DependencySets.FirstOrDefault(d => d.TargetFramework == framework);
                    result.Add(new NugetVersionInfo(framework, latestPackage.Identity.Version, dependencies.Packages.Select(p => new NugetDependencyInfo(p.Id, p.VersionRange)).ToList()));
                }
            }
            return result;
        }

        internal class NugetVersionInfo
        {
            public NugetVersionInfo(NuGetFramework frameworkVersion, NuGetVersion packageVersion, List<NugetDependencyInfo> dependencies)
            {
                FrameworkVersion = frameworkVersion;
                PackageVersion = packageVersion;
                Dependencies = dependencies;
            }

            public NuGetFramework FrameworkVersion { get; }
            public NuGetVersion PackageVersion { get; }

            public List<NugetDependencyInfo> Dependencies { get; }

        }

        internal class NugetDependencyInfo
        {
            public NugetDependencyInfo(string packageName, VersionRange version)
            {
                PackageName = packageName;
                Version = version;
            }

            public string PackageName { get; }
            public VersionRange Version { get; } 

        }
    }
}
