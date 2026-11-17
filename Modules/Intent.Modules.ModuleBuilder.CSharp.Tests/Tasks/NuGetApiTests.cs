using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.ModuleBuilder.CSharp.Tasks;
using NSubstitute;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Shouldly;
using Xunit;

namespace Intent.Modules.ModuleBuilder.CSharp.Tests.Tasks;

public class NuGetApiTests
{
    private static readonly NuGetFramework Net10 = NuGetFramework.Parse(".NETCoreApp,Version=v10.0");
    private static readonly NuGetFramework Net11 = NuGetFramework.Parse(".NETCoreApp,Version=v11.0");

    [Fact]
    public void FrameworksIncludeNet11()
    {
        NuGetApi.Frameworks.ShouldContain(Net11);
    }

    [Fact]
    public void PackageWithNet11DependencyGroupGetsAV110Entry()
    {
        var metadata = new[]
        {
            Package("Some.Package", "10.0.2", "net10.0"),
            Package("Some.Package", "11.0.0", "net11.0", "net10.0"),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        var net11 = result.SingleOrDefault(x => x.FrameworkVersion == Net11);
        net11.ShouldNotBeNull();
        net11.PackageVersion.ShouldBe(NuGetVersion.Parse("11.0.0"));
        result.First().FrameworkVersion.ShouldBe(Net11);
    }

    [Fact]
    public void PackageWithOnlyNet10DependencyGroupGetsNoV110Entry()
    {
        var metadata = new[]
        {
            Package("Some.Package", "9.0.5", "net9.0"),
            Package("Some.Package", "10.0.2", "net10.0"),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        result.ShouldNotContain(x => x.FrameworkVersion == Net11);
        result.Single(x => x.FrameworkVersion == Net10).PackageVersion.ShouldBe(NuGetVersion.Parse("10.0.2"));
    }

    // Microsoft.EntityFrameworkCore.Tools 11.0.0 declares a single, empty net10.0 group and no net11.0 group.
    [Fact]
    public void ReleaseTrainPackageWhoseNewMajorOnlyDeclaresTheOlderFrameworkStaysOnItsOwnMajorPerFramework()
    {
        var metadata = new[]
        {
            Package("Microsoft.EntityFrameworkCore.Tools", "10.0.9", ("net8.0", []), ("net9.0", []), ("net10.0", ["Microsoft.EntityFrameworkCore.Design"])),
            Package("Microsoft.EntityFrameworkCore.Tools", "10.0.15", ("net8.0", []), ("net9.0", []), ("net10.0", ["Microsoft.EntityFrameworkCore.Design"])),
            Package("Microsoft.EntityFrameworkCore.Tools", "11.0.0", ("net10.0", [])),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        var net10 = result.Single(x => x.FrameworkVersion == Net10);
        net10.PackageVersion.ShouldBe(NuGetVersion.Parse("10.0.15"));
        net10.Dependencies.Select(d => d.PackageName).ShouldBe(["Microsoft.EntityFrameworkCore.Design"]);
        var net11 = result.Single(x => x.FrameworkVersion == Net11);
        net11.PackageVersion.ShouldBe(NuGetVersion.Parse("11.0.0"));
        net11.Dependencies.ShouldBeEmpty();
    }

    [Fact]
    public void ReleaseTrainPackageWithItsOwnNet11GroupKeepsOneEntryPerMajor()
    {
        var metadata = new[]
        {
            Package("Microsoft.EntityFrameworkCore", "10.0.15", ("net10.0", ["Microsoft.EntityFrameworkCore.Abstractions"])),
            Package("Microsoft.EntityFrameworkCore", "11.0.0", ("net11.0", ["Microsoft.EntityFrameworkCore.Abstractions"])),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        result.Single(x => x.FrameworkVersion == Net11).PackageVersion.ShouldBe(NuGetVersion.Parse("11.0.0"));
        result.Single(x => x.FrameworkVersion == Net10).PackageVersion.ShouldBe(NuGetVersion.Parse("10.0.15"));
    }

    // Microsoft.EntityFrameworkCore 7.0.20 also declares only an older framework (net6.0), but newer majors declare
    // newer frameworks, so it is the net6.0 version and does not get a net7.0 entry of its own.
    [Fact]
    public void NewMajorDeclaringOnlyAnOlderFrameworkBelowThePackagesNewestStaysOnThatFramework()
    {
        var metadata = new[]
        {
            Package("Microsoft.EntityFrameworkCore", "6.0.36", ("net6.0", [])),
            Package("Microsoft.EntityFrameworkCore", "7.0.20", ("net6.0", [])),
            Package("Microsoft.EntityFrameworkCore", "8.0.22", ("net8.0", [])),
            Package("Microsoft.EntityFrameworkCore", "11.0.0", ("net11.0", [])),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        result.Single(x => x.FrameworkVersion == NuGetFramework.Parse("net6.0")).PackageVersion.ShouldBe(NuGetVersion.Parse("7.0.20"));
        result.ShouldNotContain(x => x.FrameworkVersion == NuGetFramework.Parse("net7.0"));
    }

    // ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly stopped at 7.0.10, which declares only net6.0. Only the
    // newest framework can be ahead of a package's groups, so net7.0 gets no entry and net6.0 keeps 7.0.10.
    [Fact]
    public void PackageThatStoppedBeforeTheNewestFrameworkKeepsItsNewestVersionOnItsDeclaredFramework()
    {
        var metadata = new[]
        {
            Package("ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly", "6.0.11", ("net6.0", ["Microsoft.EntityFrameworkCore.SqlServer"])),
            Package("ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly", "7.0.10", ("net6.0", ["Microsoft.EntityFrameworkCore.SqlServer"])),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        result.Single(x => x.FrameworkVersion == NuGetFramework.Parse("net6.0")).PackageVersion.ShouldBe(NuGetVersion.Parse("7.0.10"));
        result.ShouldNotContain(x => x.FrameworkVersion == NuGetFramework.Parse("net7.0"));
    }

    [Fact]
    public void PackageWithNoMajor11VersionGetsNoCopyOfItsNet10EntryAsV110()
    {
        var metadata = new[]
        {
            Package("Oracle.EntityFrameworkCore", "9.23.80", ("net8.0", [])),
            Package("Oracle.EntityFrameworkCore", "10.23.26400", ("net10.0", ["Oracle.ManagedDataAccess.Core"])),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        result.ShouldNotContain(x => x.FrameworkVersion == Net11);
        result.Single(x => x.FrameworkVersion == Net10).PackageVersion.ShouldBe(NuGetVersion.Parse("10.23.26400"));
    }

    // Fetch Latest asks nuget.org for stable versions only, so a package whose 11.0.0 is still a release candidate
    // (Npgsql.EntityFrameworkCore.PostgreSQL 11.0.0-rc.3) arrives here with its 10.x versions alone.
    [Fact]
    public void ReleaseTrainPackageWhose11IsOnlyAReleaseCandidateGetsNoV110Entry()
    {
        var metadata = new[]
        {
            Package("Npgsql.EntityFrameworkCore.PostgreSQL", "10.0.2", ("net10.0", ["Npgsql"])),
            Package("Npgsql.EntityFrameworkCore.PostgreSQL", "10.0.3", ("net10.0", ["Npgsql"])),
        };

        var result = NuGetApi.GetLatestVersionPerFramework(metadata, NuGetApi.Frameworks);

        result.ShouldNotContain(x => x.FrameworkVersion == Net11);
        result.Single(x => x.FrameworkVersion == Net10).PackageVersion.ShouldBe(NuGetVersion.Parse("10.0.3"));
    }

    private static IPackageSearchMetadata Package(string id, string version, params string[] dependencyGroupFrameworks)
    {
        return Package(id, version, dependencyGroupFrameworks.Select(f => (f, Array.Empty<string>())).ToArray());
    }

    private static IPackageSearchMetadata Package(string id, string version, params (string Framework, string[] Dependencies)[] dependencyGroups)
    {
        var package = Substitute.For<IPackageSearchMetadata>();
        package.Identity.Returns(new PackageIdentity(id, NuGetVersion.Parse(version)));
        package.DependencySets.Returns(dependencyGroups
            .Select(g => new PackageDependencyGroup(
                NuGetFramework.Parse(g.Framework),
                g.Dependencies.Select(d => new PackageDependency(d, VersionRange.Parse("1.0.0"))).ToList()))
            .ToList());
        return package;
    }
}
