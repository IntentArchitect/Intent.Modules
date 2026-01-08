#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.Utils;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static Intent.ModuleBuilder.CSharp.Api.PackageVersionModelStereotypeExtensions.PackageVersionSettings;

namespace Intent.Modules.ModuleBuilder.CSharp.Tasks
{
    internal class NuGetApi
    {
        private static readonly IReadOnlyList<NuGetFramework> Frameworks;
        private static readonly IReadOnlyList<NuGetFramework> FallBackFrameworks = [NuGetFramework.Parse("Any,Version=v0.0")];

        static NuGetApi()
        {
            Frameworks = GetFrameworks();
        }

        /// <summary>
        /// By reading these from the api generated for the enum, it will automatically pick up new
        /// .NET Frameworks added to the stereotype option rather than having to remember to update
        /// another list in here.
        /// </summary>
        private static ReadOnlyCollection<NuGetFramework> GetFrameworks()
        {
            var enumNames = Enum.GetNames<MinimumTargetFrameworkOptionsEnum>();
            var list = new List<NuGetFramework>(enumNames.Length);

            foreach (var name in enumNames)
            {
                var number = name.Split("V").LastOrDefault();
                if (string.IsNullOrWhiteSpace(number))
                {
                    continue;
                }

                var majorMinorVersion = $"{number[..^1]}.{number[^1]}";

                if (name.StartsWith("NETStandardVersionV"))
                {
                    list.Add(NuGetFramework.Parse($".NETStandard,Version=v{majorMinorVersion}"));

                }
                else if (name.StartsWith("NETCoreAppVersionV"))
                {
                    list.Add(NuGetFramework.Parse($".NETCoreApp,Version=v{majorMinorVersion}"));
                }
            }

            return list
                .OrderBy(x => x.Version.Major)
                .ThenBy(x => x.Version.Minor)
                .ToArray()
                .AsReadOnly();
        }

        public static async Task<List<NugetVersionInfo>> GetLatestVersionsForFrameworksAsync(string packageName)
        {
            Logging.Log.Info($"Executing: Fetching Package Details : {packageName}");
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());

            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);

            var packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var searchMetadata = (await packageMetadataResource.GetMetadataAsync(
                packageName,
                includePrerelease: false,
                includeUnlisted: false,
                new SourceCacheContext(),
                NullLogger.Instance,
                CancellationToken.None)).ToArray();

            if (!searchMetadata.Any())
            {
                //Try pre-releases
                searchMetadata = (await packageMetadataResource.GetMetadataAsync(packageName, includePrerelease: true, includeUnlisted: false, new SourceCacheContext(), NullLogger.Instance, CancellationToken.None)).ToArray();
            }

            var versionsForFrameworks = GetLatestVersionPerFramework(searchMetadata, Frameworks);

            if (!versionsForFrameworks.Any())
            {
                versionsForFrameworks = GetLatestVersionPerFramework(searchMetadata, FallBackFrameworks, forceOne: true);
            }

            return versionsForFrameworks;
        }

        private static List<NugetVersionInfo> GetLatestVersionPerFramework(IReadOnlyList<IPackageSearchMetadata>? searchMetadata, IReadOnlyList<NuGetFramework> frameworks, bool forceOne = false)
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
                    if (dependencies != null)
                    {
                        result.Add(new NugetVersionInfo(framework, latestPackage.Identity.Version, dependencies.Packages.Select(p => new NugetDependencyInfo(p.Id, p.VersionRange)).ToList()));
                    }
                }
            }
            if (forceOne && result.Count == 0)
            {
                var latestPackage = searchMetadata?.OrderByDescending(p => p.Identity.Version).FirstOrDefault();
                if (latestPackage != null)
                {
                    result.Add(new NugetVersionInfo(FallBackFrameworks[0], latestPackage.Identity.Version, new List<NugetDependencyInfo>()));
                }
            }

            result = result.OrderBy(n => n.FrameworkVersion.Version).ToList();
            //This is to deal with where newer version of the package infer working for example
            // MediatR 12.4.1 is configured for 6.0 and 2.0
            // MediatR 11.1.1 is configured for 2.1
            // if you are running 2.1 you can use 12.4.1 because it works for 2.0
            // Step 2: Remove entries where a higher framework has a lower version than a lower framework
            var i = 0;
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
