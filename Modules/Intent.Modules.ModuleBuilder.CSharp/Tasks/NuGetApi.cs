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
            
            if (!searchMetadata.Any())
            {
                //Try pre-releases
                searchMetadata = await packageMetadataResource.GetMetadataAsync(packageName, includePrerelease: true, includeUnlisted: false, new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);
            }

            var versionsForFrameworks = GetLatestVersionPerFramework(searchMetadata, _frameworks);

            if (!versionsForFrameworks.Any())
            {
                versionsForFrameworks = GetLatestVersionPerFramework(searchMetadata, _fallBackFrameworks, forceOne: true);
            }

            return versionsForFrameworks;
        }

        private static List<NugetVersionInfo> GetLatestVersionPerFramework(IEnumerable<IPackageSearchMetadata>? searchMetadata, List<NuGetFramework> frameworks, bool forceOne = false)
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
            if (forceOne && result.Count == 0)
            {
                var latestPackage = searchMetadata?
                    .OrderByDescending(p => p.Identity.Version)
                    .FirstOrDefault();
                result.Add(new NugetVersionInfo(_fallBackFrameworks[0], latestPackage.Identity.Version, new List<NugetDependencyInfo>()));
            }

            result = result.OrderBy(n => n.FrameworkVersion.Version).ToList();
            //This is to deal with where newer version of the package infer working for example
            // MediatR 12.4.1 is configured for 6.0 and 2.0
            // MediatR 11.1.1 is configured for 2.1
            // if you are running 2.1 you can use 12.4.1 because it works for 2.0
            // Step 2: Remove entries where a higher framework has a lower version than a lower framework
            int i = 0;
            while (i < result.Count - 1)
            {
                var current = result[i];
                var next = result[i + 1];

                // Compare only if the previous framework is lower than the current framework
                if (current.FrameworkVersion.Version < next.FrameworkVersion.Version &&
                    current.PackageVersion.Version > next.PackageVersion.Version)
                {
                    result.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }
            //Latest Framework first
            result.Reverse();
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
