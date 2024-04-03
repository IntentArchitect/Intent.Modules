#!/usr/bin/env pwsh

# "dotnet pack" all projects. We want to dynamically work out which projects to pack so that if
# new projects to pack are added to the repository, then they will also be included automatically
# as opposed to us needing to update a list somewhere.
#
# It seemed easiest to me to work out compilation order based on project dependencies using C# due
# to my familiarity with it. We also already use .NET Core, so it's not needlessly adding a new
# dependency anyway. The following guide was used to work out how to use C# from PowerShell:
# https://blog.adamfurmanek.pl/2016/03/19/executing-c-code-using-powershell-script/
#
# Finally, we manually call "dotnet pack" in the correct order because Azure DevOps DotNetCoreCLI@2
# task with `command: 'pack'` unfortunately doesn't work for us because regardless of the order of
# the .csproj files in `searchPatternPack`, it will always process the projects ordered by their
# full path, but we need projects to be packed in the order of their dependencies.

param(
    [string]$dotnetPackOutputDirectory,
    [string]$vstsFeedUrl,
    [string]$workingDirectory = $(Get-Location),
    # https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#system-variables-devops-services
    [bool]$isOnBuildAgent = $($env:TF_BUILD -eq "True")
)

Write-Host "`$dotnetPackOutputDirectory=$dotnetPackOutputDirectory"
Write-Host "`$vstsFeedUrl=$vstsFeedUrl"
Write-Host "`$workingDirectory=$workingDirectory"
Write-Host "`$isOnBuildAgent=$isOnBuildAgent"

$id = [System.Guid]::NewGuid().ToString("N")
$code = @"
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    namespace Intent$id
    {
        public class Program
        {
            public static string[] Main(string directory)
            {
                Console.WriteLine($"Scanning {directory}...");

                var files = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);

                var projects = files
                    .Select(GetDetail)
                    .Where(x => x != null)
                    .Select(x => x.Value)
                    .ToArray();

                var stillToOrderForCompilation = projects
                    .Select(item => (
                        item.fullPath,
                        item.packageId,
                        referencedPackageNames: item.referencedPackageNames
                            .Where(packageName => projects.Any(project => project.packageId == packageName))
                            .ToArray())
                    )
                    .ToList();

                var orderedForCompilation = new List<(string fullPath, string packageId, IReadOnlyCollection<string> referencedPackageNames)>();
                var handledPackageIds = new HashSet<string>();

                while (stillToOrderForCompilation.Any())
                {
                    var toUpdate = stillToOrderForCompilation
                        .Where(item => item.referencedPackageNames.All(handledPackageIds.Contains))
                        .OrderBy(x => x.packageId)
                        .ToArray();

                    if (!toUpdate.Any())
                    {
                        throw new Exception("");
                    }

                    foreach (var item in toUpdate)
                    {
                        Console.WriteLine($"Adding {item.packageId}");
                        orderedForCompilation.Add(item);
                        handledPackageIds.Add(item.packageId);
                        stillToOrderForCompilation.Remove(item);
                    }
                }

                return orderedForCompilation.Select(x => x.fullPath).ToArray();
            }

            private static (string fullPath, string packageId, IReadOnlyCollection<string> referencedPackageNames)? GetDetail(string path)
            {
                var xDocument = XDocument.Load(path);

                // If the project file doesn't contain a Version or PackageVersion element, then
                // we assume it's not intended to be packed.
                if (xDocument.XPathSelectElement(".//Version") == null &&
                    xDocument.XPathSelectElement(".//PackageVersion") == null)
                {
                    return null;
                }

                var packageId = xDocument.XPathSelectElement(".//PackageId")?.Value ??
                                Path.GetFileNameWithoutExtension(path);

                var dependencies = xDocument
                    .XPathSelectElements(".//PackageReference")
                    .Select(x => x.Attribute("Include")!.Value.ToLowerInvariant())
                    .ToArray();

                return (path, packageId.ToLowerInvariant(), dependencies);
            }
        }
    }
"@
Add-Type -TypeDefinition $code -Language CSharp
Invoke-Expression "`$projects = [Intent$id.Program]::Main(`"$workingDirectory`")"

if ($isOnBuildAgent) {
    Write-Host "Detected that script is running in build task"
    $packOutputParam = " --output $dotnetPackOutputDirectory";

    # Even though pack will be run in dependency order, the build can still fail if a package
    # version was built for the first time within the same run unless we tell NuGet to also check
    # build output during restore.
    Set-Content $(Join-Path $workingDirectory "NuGet.Config") @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="dotnet pack output directory" value="$dotnetPackOutputDirectory" />
        <!-- Requires "NuGetAuthenticate@0" task to be called first in Azure Pipelines -->
        <add key="vsts feed" value="$vstsFeedUrl" />
    </packageSources>
</configuration>
"@
}

$index = 1
foreach ($project in $projects) {
    Write-Host
    Write-Host "Running dotnet clean for $project ($index of $($projects.length))..."
    Invoke-Expression "dotnet clean $project --verbosity quiet --nologo"
    $index++
}

$index = 1
foreach ($project in $projects) {
    Write-Host
    Write-Host "Running dotnet build and pack for $project ($index of $($projects.length))..."
    Invoke-Expression "dotnet restore $project --verbosity quiet"
    Invoke-Expression "dotnet build $project --verbosity quiet --nologo /property:WarningLevel=0 --configuration Release"
    Invoke-Expression "dotnet pack $project --verbosity quiet --nologo  /property:WarningLevel=0$packOutputParam"
    $index++
}

# Resolves the following issue which would often get logged and cause this task to in Azure DevOps to become stuck:
# The STDIO streams did not close within 10 seconds of the exit event from process '/usr/bin/pwsh'. This may indicate a child process inherited the STDIO streams and has not yet exited.
if ($isOnBuildAgent) {
    Write-Host
    Write-Host "Stopping dotnet processes"
    Get-Process | Where-Object { $_.ProcessName.StartsWith("dotnet", "InvariantCultureIgnoreCase") } | Foreach-Object { Stop-Process $_ }
}
