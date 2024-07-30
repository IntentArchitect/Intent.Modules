#!/usr/bin/env pwsh

param(
    [string]$buildArtifactStagingDirectory,
    [string]$testsIntentSolutionRelativePath
)

$repoConfigContent = 
"<?xml version=""1.0"" encoding=""utf-8""?>
<assetRepositories>
  <entries>
    <entry>
      <name>Pipeline build artifact staging directory</name>
      <address>$buildArtifactStagingDirectory</address>
      <isBuiltIn>false</isBuiltIn>
      <order>3</order>
    </entry>
  </entries>
</assetRepositories>"

$moduleLookup = @{}
$moduleFileNames = Get-ChildItem "$buildArtifactStagingDirectory/*.imod" | % {
    $file = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
    $dotNumber = 0
    $dotIndex = -1
    $dotIndexNo3 = -1
    for ($i = $file.Length - 1; $i -gt 0; $i--) {
        $cur = $file[$i]
        if ($cur -eq '.') {
            $dotNumber++
            $dotIndex = $i
        }
        if (($dotNumber -eq 3) -and ([System.Char]::IsDigit($file[$i + 1]))) {
            $dotIndexNo3 = $dotIndex
        }
        if ($dotNumber -eq 4) {
            if ([System.Char]::IsDigit($file[$i + 1])) {
                $dotIndex = $i
            } else {
                $dotIndex = $dotIndexNo3
            }
            break
        }
    }

    if ($dotIndex -gt -1) {
        $moduleLookup.Add($file.Substring(0, $dotIndex), $file.Substring($dotIndex + 1))
    }
}

$curLocation = Get-Location;
Write-Host "`$curLocation = $curLocation"

$testSln = [xml] (Get-Content "./$testsIntentSolutionRelativePath" -Encoding UTF8)
$testSlnDir = [System.IO.Path]::GetDirectoryName($testsIntentSolutionRelativePath)
Write-Host "`$testSlnDir = $testSlnDir"

$repoPath = [System.IO.Path]::Combine($curLocation, $testSlnDir, "intent.repositories.config")
Write-Host "`$repoPath = $repoPath"
$repoConfigContent | Set-Content $repoPath -Encoding UTF8

$testSln.solution.applications.application | % {
    $appRelPath = [System.IO.Path]::Combine($curLocation, $testSlnDir, $_.relativePath)
    $basePath = [System.IO.Path]::GetDirectoryName($appRelPath)
    $modulesConfig = [System.IO.Path]::Combine($basePath, "modules.config")

    $modulesConfigContent = [xml] (Get-Content $modulesConfig -Encoding UTF8)
    $changed = $false
    $modulesConfigContent.modules.module | % { 
        $module = $_
        $moduleVersionFound = $moduleLookup[$module.moduleId]
        if ($moduleVersionFound -ne $null) { 
            $module.version = $moduleVersionFound
            $changed = $true
        }
    }
    if ($changed) {
        $modulesConfigContent.Save($modulesConfig)
    }
}
