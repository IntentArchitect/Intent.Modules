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
        internal static readonly IReadOnlyList<NuGetFramework> Frameworks;
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

        internal static List<NugetVersionInfo> GetLatestVersionPerFramework(IReadOnlyList<IPackageSearchMetadata>? searchMetadata, IReadOnlyList<NuGetFramework> frameworks, bool forceOne = false)
        {
            var result = new List<NugetVersionInfo>();
            var releaseTrainAhead = GetReleaseTrainAheadOfDeclaredFrameworks(searchMetadata, frameworks);
            foreach (var framework in frameworks)
            {
                if (releaseTrainAhead?.Info.FrameworkVersion == framework)
                {
                    result.Add(releaseTrainAhead.Value.Info);
                    continue;
                }

                var latestPackage = searchMetadata?
                    .Where(p => releaseTrainAhead?.Versions.Contains(p.Identity.Version) != true)
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

        /// <summary>
        /// A package that follows the .NET release train can ship the major for a new .NET release before it
        /// declares that release's framework: Microsoft.EntityFrameworkCore.Tools 11.0.0 declares only net10.0.
        /// Matched by dependency group alone, 11.0.0 would become the net10.0 version and net11.0 would get none.
        /// So the newest framework, when it is above every one the package declares, takes the newest version of its
        /// own major whose nearest group is the package's newest declared one, and those versions are kept off the
        /// older frameworks. Only the newest framework: a package that stopped declaring new frameworks years ago
        /// (ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly 7.0.10 declares only net6.0) is not a train running
        /// ahead, and its existing entries must stay as they are.
        /// </summary>
        private static (NugetVersionInfo Info, HashSet<NuGetVersion> Versions)? GetReleaseTrainAheadOfDeclaredFrameworks(
            IReadOnlyList<IPackageSearchMetadata>? searchMetadata,
            IReadOnlyList<NuGetFramework> frameworks)
        {
            var newestDeclared = searchMetadata?
                .SelectMany(p => p.DependencySets.Select(d => d.TargetFramework))
                .Where(IsDotNetRelease)
                .MaxBy(f => f.Version);
            var newestFramework = frameworks.Where(IsDotNetRelease).MaxBy(f => f.Version);
            if (searchMetadata == null || newestDeclared == null || newestFramework == null || newestFramework.Version <= newestDeclared.Version)
            {
                return null;
            }

            // Only a package whose own major matches the newest framework it declares follows the train.
            var followsReleaseTrain = searchMetadata.Any(p =>
                p.Identity.Version.Major == newestDeclared.Version.Major &&
                p.DependencySets.Any(d => d.TargetFramework == newestDeclared));
            if (!followsReleaseTrain)
            {
                return null;
            }

            var reducer = new FrameworkReducer();
            var candidates = searchMetadata
                .Where(p => p.Identity.Version.Major == newestFramework.Version.Major &&
                            reducer.GetNearest(newestFramework, p.DependencySets.Select(d => d.TargetFramework)) == newestDeclared)
                .ToList();
            var latestPackage = candidates.MaxBy(p => p.Identity.Version);
            if (latestPackage == null)
            {
                return null;
            }

            var dependencies = latestPackage.DependencySets.First(d => d.TargetFramework == newestDeclared);
            return (
                new NugetVersionInfo(newestFramework, latestPackage.Identity.Version, dependencies.Packages.Select(p => new NugetDependencyInfo(p.Id, p.VersionRange)).ToList()),
                candidates.Select(p => p.Identity.Version).ToHashSet());
        }

        private static bool IsDotNetRelease(NuGetFramework framework)
        {
            return framework.Framework == FrameworkConstants.FrameworkIdentifiers.NetCoreApp && framework.Version.Major >= 5;
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
