class ImodSpecInfo {
    [string]$FilePath
    [string]$DirectoryPath
    [string]$Id
    [VersionInfo]$Version
    [string]$Authors
    [string]$Summary
    [string]$Description
    [Boolean]$IsObsolete
    [Dependency[]]$Dependencies
    [string]$ReleaseNotes
    [string]$Tags

    ImodSpecInfo([string]$filePath, [xml]$content) {
        $this.FilePath = $filePath
        $this.DirectoryPath = [System.IO.Path]::GetDirectoryName($filePath)
        $this.Id = $content.package.id
        $this.Version = if (-not [System.String]::IsNullOrWhiteSpace($content.package.version)) { [VersionInfo]::new($content.package.version) } else { $null }
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
    [VersionInfo]$Version

    Dependency(
        [string]$Id,
        [string]$Version
    ) {
        $this.Id = $Id
        $this.Version = if (-not [System.String]::IsNullOrWhiteSpace($Version)) { [VersionInfo]::new($Version) } else { $null }
    }
}

class VersionInfo {
    [string]$OriginalVersion
    [int]$Major = 0
    [int]$Minor = 0
    [int]$Build = 0
    [string]$Revision = $null

    VersionInfo([string]$versionString) {
        $this.OriginalVersion = $versionString
        $this.ExtractVersionParts($versionString)
    }

    [void]ExtractVersionParts([string]$versionString) {
        $pattern = '^(\d+)\.(\d+)\.(\d+)(?:-([A-Za-z0-9.]+))?$'
        
        if ($versionString -match $pattern) {
            $this.Major = [int]$matches[1]
            $this.Minor = [int]$matches[2]
            $this.Build = [int]$matches[3]
            if ($matches[4]) {
                $this.Revision = $matches[4]
            }
        } else {
            Write-Host "Version format not recognized: $($versionString)."
        }
    }

    [string]ToString() {
        if ($null -ne $this.Revision) {
            return "$($this.Major).$($this.Minor).$($this.Build)-$($this.Revision)"
        } else {
            return "$($this.Major).$($this.Minor).$($this.Build)"
        }
    }
}

function PrintError {
    param (
        [string]$Message,
        [Boolean]$DevOps
    )
    if ($DevOps) {
        Write-Host "##vso[task.logissue type=error;]$($Message)"
    } else {
        Write-Host "$($Message)" -ForegroundColor Red
    }
}

function PrintWarning {
    param (
        [string]$Message,
        [Boolean]$DevOps
    )
    if ($DevOps) {
        Write-Host "##vso[task.logissue type=warning;]$($Message)"
    } else {
        Write-Host "$($Message)" -ForegroundColor DarkYellow
    }
}

function PrintAffectedFileName {
    param (
        [string]$Name,
        [Boolean]$DevOps
    )
    if ($DevOps) {
        Write-Host "##[section]$($Name)"
    } else {
        Write-Host "# $($Name)" -ForegroundColor Green
    }
}

$DevOps = [bool]$args[0]
$imodSpecInfos = New-Object 'System.Collections.Generic.Dictionary[string, ImodSpecInfo]'
$global:hasError = $false

Get-ChildItem -Path Modules -Filter *.imodspec -Recurse -Depth 2 | ForEach-Object {
    $file = $_.FullName
    
    [xml]$content = Get-Content $file

    $spec = [ImodSpecInfo]::new($_.FullName, $content)
    if ($imodSpecInfos.ContainsKey($spec.Id)) {
        PrintError "There is a duplicate Module Id found: $($spec.Id)" $DevOps
        Write-Host "  - $($file)"
        Write-Host "  - $($imodSpecInfos[$spec.Id].FilePath)"
        $global:hasError = $true
        $global:hasError > $null # Quench the warning since this is being used in other parts of the script
    } else {
        $imodSpecInfos.Add($spec.Id, $spec)
    }
}

$validationRules = @{
    Authors = { 
        param([ImodSpecInfo]$info) 
        if ([string]::IsNullOrEmpty($info.Authors) -or ($info.Authors -like "Intent.*")) {
            return "Authors not set" 
        } 
    }
    Summary = { 
        param([ImodSpecInfo]$info) 
        if ([string]::IsNullOrEmpty($info.Summary) -or ($info.Summary -like "A custom module for*")) {
            return "Summary not set" 
        } 
    }
    Description = { 
        param([ImodSpecInfo]$info) 
        if ([string]::IsNullOrEmpty($info.Description) -or ($info.Description -like "A custom module for*")) {
            return "Description not set" 
        } 
    }
    Obsolete = { 
        param([ImodSpecInfo]$info)
        $obsoleteDependencies = $info.Dependencies | Where-Object { 
            $imodSpecInfos.ContainsKey($_.Id) -and $imodSpecInfos[$_.Id].IsObsolete 
        }
        if ($obsoleteDependencies) {
            return "Contains obsolete dependencies: $($obsoleteDependencies.Id -join ', ')"
        }
    }
    HasVersion = {
        param([ImodSpecInfo]$info)
        if ($info.Version -eq $null) {
            return "Does not have a version specified"
        }
        if ($info.Dependencies -ne $null) {
            $missingVersions = @($info.Dependencies | Where-Object {
                $_.Version -eq $null
            })
            if ($missingVersions.Count -gt 0) {
                return "Dependencies with missing versions: $($missingVersions.Id -join ', ')"
            }
        }
    }
    HasReleaseNotes = { 
        param([ImodSpecInfo]$info) 
        if ($info.ReleaseNotes -ne $null -and [System.String]::IsNullOrWhiteSpace($info.ReleaseNotes)) { 
            return "No release notes defined" 
        } 
    }
    MissingVersionInReleaseNotes = { 
        param([ImodSpecInfo]$info)
        if ([System.String]::IsNullOrWhiteSpace($info.ReleaseNotes)) { 
            return $null 
        }
        $releaseNoteFile = [System.IO.Path]::Combine($info.DirectoryPath, $info.ReleaseNotes)
        if (-not [System.IO.File]::Exists($releaseNoteFile)) {
            return "Could not locate release notes file: $($releaseNoteFile)"
        }
        [string]$releaseNotesContent = Get-Content $releaseNoteFile
        $targetVersion = "$($info.Version.Major).$($info.Version.Minor).$($info.Version.Build)"
        if (-not $releaseNotesContent.Contains($targetVersion)) {
            return "Could not locate version number $($targetVersion) in release notes"
        }
    }
}

$validationWarnings = @{
    Tags = {
        param([ImodSpecInfo]$info)
        if (-not [System.String]::IsNullOrWhiteSpace($info.Tags)) {
            return "No tags specified"
        }
    }
}

foreach ($info in $imodSpecInfos.Values) {
    $reportedFileName = $false

    foreach ($validation in $validationRules.GetEnumerator()) {
        $result = & $validation.Value $info
        if ($result) {
            if (-not $reportedFileName) {
                PrintAffectedFileName "$($info.FilePath)" $DevOps
                $reportedFileName = $true
            }
            PrintError " - $($result)" $DevOps
            $global:hasError = $true
        }
    }

    foreach ($validation in $validationWarnings.GetEnumerator()) {
        $result = & $validation.Value $info
        if ($result) {
            if (-not $reportedFileName) {
                PrintAffectedFileName "$($info.FilePath)" $DevOps
                $reportedFileName = $true
            }
            PrintWarning " - $($result)" $DevOps
        }
    }
}

if ($global:hasError) {
    Write-Error "Imodspec validation failed."
    exit 1;
}
