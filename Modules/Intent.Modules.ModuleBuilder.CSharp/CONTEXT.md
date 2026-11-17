# Context: Intent.ModuleBuilder.CSharp

## Purpose
Module Builder support for C# and Razor templates, plus the `NuGet Packages` designer elements and the
"Fetch Latest" task that fills in each package's per-framework versions from nuget.org.

## Invariants & Constraints
- **Fetch Latest's framework list is the `Minimum Target Framework` option list.** `NuGetApi.Frameworks` is
  built from the generated `MinimumTargetFrameworkOptionsEnum` names (`NETCoreAppVersionV110` → `net11.0`), so
  supporting a new .NET version is one new option on the `Package Version Settings` stereotype plus a
  regeneration (3.8.1 added v11.0). No second list to keep in step.
- **A framework entry comes from a nuspec dependency group, not a `lib/` folder.**
  `NuGetApi.GetLatestVersionPerFramework` matches `IPackageSearchMetadata.DependencySets` by exact target
  framework, so a package that ships `lib/net11.0` with no `net11.0` dependency group gets no v11.0 entry; its
  older entry still covers net11 projects as a lower bound. `NuGetApiTests` pins this with fake metadata.
  One exception (3.8.1): the **release-train-ahead rule**.
- **A release-train package can ship a new .NET's major before declaring that framework.**
  `Microsoft.EntityFrameworkCore.Tools` 11.0.0 declares only an empty `net10.0` group. Matched by group alone,
  it became the net10 version (net10 apps would get Tools 11 beside EF Core 10, without Design) and net11 got
  nothing. So when the newest framework in the list is above every framework a package declares, and the
  package's own major matches its newest declared framework (it follows the train), the newest framework takes
  the newest version of its own major whose nearest group is that newest declared one; those versions are kept
  off the older frameworks, so net10 stays on 10.x.
  - Decided (user, 2026-11-10): framework without its own group takes the newest same-major version from the
    nearest lower group, applied both ways (net10 stays on 10.x).
  - Narrowed to the **newest framework only**, checked against live nuget.org data for 30 packages: applied to
    every framework, `ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly` 7.0.10 (declares only net6.0)
    would move its net6 entry to 6.0.11 and add a net7 entry, and Pomelo 9.0.0 (declares only net8.0) likewise.
    A package that stopped declaring new frameworks is not a train running ahead. `EF Core` 7.0.20 (net6.0 only)
    is untouched either way, because the package declares net11.0 in a later major.
  - Rejected: telling Tools and EF Core 7 apart from metadata alone. Both are "a new major declaring only the
    previous framework"; only the position of the framework in the list separates them.
- **`NuGetApi.Frameworks` and `GetLatestVersionPerFramework` are `internal` only for the test project**
  (`InternalsVisibleTo` `Intent.Modules.ModuleBuilder.CSharp.Tests`, 3.8.1). The user chose that over testing
  the private members through reflection, which would break silently on a rename.
