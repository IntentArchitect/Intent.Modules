#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory = $true)][string]$ModulesFolder,
    [Parameter(Mandatory = $true)][string]$TestsFolder
)

$global:hasError = $false
$cwd = Get-Location

class ModuleSpecification {
    [string]$FilePath
    [string]$DirectoryPath
    [string]$Id
    [semver]$Version
    [string]$Authors
    [string]$Summary
    [string]$Description
    [Boolean]$IsObsolete
    [Dependency[]]$Dependencies
    [string]$ReleaseNotes
    [string]$Tags

    ModuleSpecification([System.IO.FileInfo]$file) {
        [xml]$content = Get-Content $file.FullName

        $this.FilePath = $file.FullName
        $this.DirectoryPath = $file.DirectoryName
        $this.Id = $content.package.id
        $this.Version = if (-not [System.String]::IsNullOrWhiteSpace($content.package.version)) { [semver]$content.package.version } else { $null }
        $this.Authors = $content.package.authors
        $this.Summary = $content.package.summary
        $this.Description = $content.package.description
        $this.IsObsolete = $this.Description.ToLower().Contains("obsolete")
        $this.Dependencies = $content.package.dependencies.dependency | Where-Object { $null -ne $_.Id } | ForEach-Object { [Dependency]::new($_.id, $_.version) }
        $this.ReleaseNotes = $content.package.releaseNotes
        $this.Tags = $content.package.tags
    }
}

class Dependency {
    [string]$Id
    [semver]$Version

    Dependency(
        [string]$Id,
        [string]$Version
    ) {
        $this.Id = $Id
        $this.Version = if (-not [System.String]::IsNullOrWhiteSpace($Version)) { [semver]$Version } else { $null }
    }
}

function PrintError {
    param (
        [string]$FilePath,
        [string]$Message
    )

    $global:hasError = $true;
    if ($global:hasError) { } # To silence warning, see https://learn.microsoft.com/en-gb/powershell/utility-modules/psscriptanalyzer/rules/usedeclaredvarsmorethanassignments?view=ps-modules

    $FilePath = [System.IO.Path]::GetRelativePath($cwd, $FilePath)

    $Message = "$($FilePath): $Message"

    if ($Env:TF_BUILD) {
        Write-Host "##vso[task.logissue type=error;]$($Message)"
    }
    else {
        Write-Host "$($Message)" -ForegroundColor Red
    }
}

Write-Host "Scanning $ModulesFolder..."
$moduleSpecificationFilePaths = (Get-ChildItem -Recurse -File "./$ModulesFolder" -Filter "*.imodspec").Where{ -not $_.DirectoryName.Replace("\", "/").Contains("/.intent/") }
$moduleSpecifications = $moduleSpecificationFilePaths | ForEach-Object { [ModuleSpecification]::new($_) }
$moduleSpecificationsById = @{};
foreach ($item in $moduleSpecifications) {
    $moduleSpecificationsById[$item.Id] = $item
}

foreach ($info in $moduleSpecifications) {
    if ([string]::IsNullOrEmpty($info.Authors) -or ($info.Authors -like "Intent.*")) {
        PrintError $info.FilePath "Authors not set"
    }

    if ([string]::IsNullOrEmpty($info.Summary) -or ($info.Summary -like "A custom module for*")) {
        PrintError $info.FilePath "Summary not set"
    }

    if ([string]::IsNullOrEmpty($info.Description) -or ($info.Description -like "A custom module for*")) {
        PrintError $info.FilePath "Description not set"
    }

    if ($null -eq $info.Version) {
        PrintError $info.FilePath "Does not have a version specified"
    }

    if ($null -ne $info.Dependencies) {
        $obsoleteDependencies = $info.Dependencies | Where-Object { $moduleSpecificationsById.ContainsKey($_.Id) -and $moduleSpecificationsById[$_.Id].IsObsolete }
        if ($obsoleteDependencies) {
            PrintError $info.FilePath "Contains obsolete dependencies: $($obsoleteDependencies.Id -join ', ')"
        }

        $missingVersions = @($info.Dependencies | Where-Object { $_.Version -eq $null })
        if ($missingVersions.Count -gt 0) {
            PrintError $info.FilePath "Has dependencies with missing versions: $($missingVersions.Id -join ', ')"
        }
    }

    if ($null -ne $info.ReleaseNotes -and [System.String]::IsNullOrWhiteSpace($info.ReleaseNotes)) {
        PrintError $info.FilePath "No release notes defined"
    }

    if (-not [System.String]::IsNullOrWhiteSpace($info.ReleaseNotes)) {
        $releaseNoteFile = [System.IO.Path]::Combine($info.DirectoryPath, $info.ReleaseNotes)

        if (-not [System.IO.File]::Exists($releaseNoteFile)) {
            PrintError $info.FilePath "Could not locate release notes file"
        }

        $targetVersion = "$($info.Version.Major).$($info.Version.Minor).$($info.Version.Patch)"
        if (-not (Get-Content $releaseNoteFile).Contains("### Version $targetVersion")) {
            $releaseNotesRelativePath = [System.IO.Path]::GetRelativePath($cwd, $releaseNoteFile);
            PrintError "$($releaseNotesRelativePath): Could not locate version number $($targetVersion) in release notes"
        }
    }

    if ([System.String]::IsNullOrWhiteSpace($info.Tags)) {
        PrintError $info.FilePath "No tags specified"
    }
}

Write-Host "Scanning $TestsFolder..."
$moduleConfigPaths = Get-ChildItem -Recurse -File "./$TestsFolder" -Filter "modules.config"

foreach ($path in $moduleConfigPaths) {
    [xml]$parsed = (Get-Content $path.FullName -Encoding UTF8)

    foreach ($module in $parsed.modules.module) {
        $id = $module.moduleId;
        $version = $module.version;

        [ModuleSpecification] $knownVersion = $moduleSpecificationsById[$id]
        if ($null -eq $knownVersion -or $knownVersion.Version -eq $version) {
            continue;
        }

        PrintError $path.FullName "Version mismatch for $($knownVersion.Id), expected $($knownVersion.Version) but found $version"
    }
}

if ($global:hasError) {
    exit -1;
}

exit 0
